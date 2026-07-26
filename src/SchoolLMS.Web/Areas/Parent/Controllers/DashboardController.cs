using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Services.Dashboards;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Web.Areas.Parent.Controllers;

[Area("Parent")]
[Authorize(Roles = "Parent,SuperAdministrator,SchoolAdministrator")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(IDashboardService dashboardService, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? childId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "لوحة تحكم ولي الأمر";
        var user = await _userManager.GetUserAsync(User);
        var guardianId = await _db.Guardians.AsNoTracking()
            .Where(x => x.UserId == user!.Id && !x.IsDeleted)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (guardianId == 0)
        {
            guardianId = await _db.Guardians.AsNoTracking().Where(x => !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
        }

        var model = guardianId == 0 ? null : await _dashboardService.GetParentDashboardAsync(guardianId, childId, cancellationToken);
        return View(model);
    }
}
