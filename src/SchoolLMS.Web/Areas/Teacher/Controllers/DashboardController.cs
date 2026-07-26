using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Services.Dashboards;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Web.Areas.Teacher.Controllers;

[Area("Teacher")]
[Authorize(Roles = "Teacher,SuperAdministrator,SchoolAdministrator")]
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

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "لوحة تحكم المعلم";
        var user = await _userManager.GetUserAsync(User);
        var teacherId = await _db.Teachers.AsNoTracking()
            .Where(x => x.UserId == user!.Id && !x.IsDeleted)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacherId == 0)
        {
            teacherId = await _db.Teachers.AsNoTracking().Where(x => !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
        }

        var model = teacherId == 0 ? null : await _dashboardService.GetTeacherDashboardAsync(teacherId, cancellationToken);
        return View(model);
    }
}
