using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Domain.Interfaces;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db,
        IAuditService audit)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _db = db;
        _audit = audit;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(x =>
            x.UserName == model.UserNameOrEmail ||
            x.Email == model.UserNameOrEmail ||
            x.PhoneNumber == model.UserNameOrEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            await LogLoginAsync(null, model.UserNameOrEmail, false, "User not found or inactive", cancellationToken);
            ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة.");
            return View(model);
        }

        if (user.MustChangePassword)
        {
            ModelState.AddModelError(string.Empty, "يجب تغيير كلمة المرور قبل تسجيل الدخول.");
            return View(model);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            await LogLoginAsync(user.Id, user.UserName, false, result.IsLockedOut ? "Locked out" : "Invalid password", cancellationToken);
            ModelState.AddModelError(string.Empty, result.IsLockedOut
                ? "تم قفل الحساب مؤقتاً بسبب محاولات فاشلة متكررة."
                : "بيانات الدخول غير صحيحة.");
            return View(model);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var schoolIds = await _db.UserSchoolAssignments.AsNoTracking()
            .Where(x => x.UserId == user.Id && x.IsActive && !x.IsDeleted)
            .Select(x => x.SchoolId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var permissionIds = await _db.UserPermissions.AsNoTracking()
            .Where(x => x.UserId == user.Id && x.IsGranted)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var roleIds = await _db.Roles.AsNoTracking()
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        var rolePermissionIds = await _db.RolePermissions.AsNoTracking()
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var permissions = await _db.Permissions.AsNoTracking()
            .Where(x => permissionIds.Contains(x.Id) || rolePermissionIds.Contains(x.Id))
            .Select(x => x.Key)
            .Distinct()
            .ToListAsync(cancellationToken);

        var claims = new List<Claim>();
        foreach (var schoolId in schoolIds)
        {
            claims.Add(new Claim("school_id", schoolId.ToString()));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await LogLoginAsync(user.Id, user.UserName, true, null, cancellationToken);
        await _audit.LogAsync("Login", nameof(ApplicationUser), user.Id, success: true, cancellationToken: cancellationToken);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (roles.Contains(AppRoles.SuperAdministrator) || roles.Contains(AppRoles.SchoolAdministrator) || roles.Contains(AppRoles.CentralAdministrator))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        if (roles.Contains(AppRoles.Teacher))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Teacher" });
        }

        if (roles.Contains(AppRoles.Parent))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Parent" });
        }

        if (roles.Contains(AppRoles.Student))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Student" });
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    public IActionResult SetLanguage(string culture, string returnUrl = "/")
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new Microsoft.AspNetCore.Localization.RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

        return LocalRedirect(returnUrl);
    }

    private async Task LogLoginAsync(string? userId, string? userName, bool success, string? reason, CancellationToken cancellationToken)
    {
        _db.LoginHistories.Add(new LoginHistory
        {
            UserId = userId,
            UserName = userName,
            Success = success,
            FailureReason = reason,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);
    }
}
