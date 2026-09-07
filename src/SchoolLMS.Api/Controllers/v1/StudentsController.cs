using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Services.Dashboards;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Api.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentsController(IDashboardService dashboardService, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("me/dashboard")]
    public async Task<IActionResult> MyDashboard(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var studentId = await _db.Students.AsNoTracking()
            .Where(x => x.UserId == user.Id && !x.IsDeleted)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == 0) return NotFound();

        var dashboard = await _dashboardService.GetStudentDashboardAsync(studentId, cancellationToken);
        return Ok(dashboard);
    }
}
