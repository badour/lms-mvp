using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Infrastructure.Identity;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Api.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly JwtTokenService _jwt;

    public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, JwtTokenService jwt)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
    }

    public record LoginRequest(string UserNameOrEmail, string Password);
    public record TokenResponse(string AccessToken, DateTime ExpiresAt, string RefreshToken);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x =>
            x.UserName == request.UserNameOrEmail ||
            x.Email == request.UserNameOrEmail ||
            x.PhoneNumber == request.UserNameOrEmail, cancellationToken);

        if (user is null || !user.IsActive || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var schoolIds = await _db.UserSchoolAssignments.AsNoTracking()
            .Where(x => x.UserId == user.Id && x.IsActive && !x.IsDeleted)
            .Select(x => x.SchoolId)
            .ToListAsync(cancellationToken);

        var claims = roles.Select(r => new Claim(ClaimTypes.Role, r))
            .Concat(schoolIds.Select(id => new Claim("school_id", id.ToString())))
            .ToList();

        var (accessToken, expires) = _jwt.CreateAccessToken(user, claims);
        var refresh = _jwt.CreateRefreshToken(user.Id, HttpContext.Connection.RemoteIpAddress?.ToString());
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new TokenResponse(accessToken, expires, refresh.Token));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        var existing = await _db.RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive || existing.User is null)
        {
            return Unauthorized();
        }

        existing.RevokedAt = DateTime.UtcNow;
        var roles = await _userManager.GetRolesAsync(existing.User);
        var schoolIds = await _db.UserSchoolAssignments.AsNoTracking()
            .Where(x => x.UserId == existing.UserId && x.IsActive && !x.IsDeleted)
            .Select(x => x.SchoolId)
            .ToListAsync(cancellationToken);

        var claims = roles.Select(r => new Claim(ClaimTypes.Role, r))
            .Concat(schoolIds.Select(id => new Claim("school_id", id.ToString())));

        var (accessToken, expires) = _jwt.CreateAccessToken(existing.User, claims);
        var next = _jwt.CreateRefreshToken(existing.UserId, HttpContext.Connection.RemoteIpAddress?.ToString());
        existing.ReplacedByToken = next.Token;
        _db.RefreshTokens.Add(next);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new TokenResponse(accessToken, expires, next.Token));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        var existing = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        if (existing is not null)
        {
            existing.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Ok();
    }
}
