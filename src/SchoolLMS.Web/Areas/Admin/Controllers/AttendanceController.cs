using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Attendance;
using SchoolLMS.Application.Services.Attendance;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AttendanceController : Controller
{
    private readonly IAttendanceAdminService _attendanceService;
    private readonly ApplicationDbContext _db;

    public AttendanceController(IAttendanceAdminService attendanceService, ApplicationDbContext db)
    {
        _attendanceService = attendanceService;
        _db = db;
    }

    public async Task<IActionResult> Index(int? schoolId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "حضور الصفوف";
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);
        var items = await _attendanceService.ListRecentAsync(schoolId, cancellationToken);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "تسجيل حضور صف";
        await LoadTeachersAsync(cancellationToken);
        return View(new AttendanceCreateForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AttendanceCreateForm form, CancellationToken cancellationToken)
    {
        var request = new SaveAttendanceRequest
        {
            TeacherId = form.TeacherId,
            AcademicStageId = form.AcademicStageId,
            ClassSectionId = form.ClassSectionId,
            AttendanceDate = form.AttendanceDate,
            Students = (form.Students ?? []).Select(x => new AttendanceStudentMarkDto
            {
                StudentId = x.StudentId,
                IsPresent = x.IsPresent
            }).ToList()
        };

        var result = await _attendanceService.SaveAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadTeachersAsync(cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم حفظ حضور الصف بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Stages(int teacherId, CancellationToken cancellationToken)
    {
        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == teacherId && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return Json(Array.Empty<object>());
        }

        var stages = await (
            from assignment in _db.TeacherAssignments.AsNoTracking()
            join section in _db.ClassSections.AsNoTracking() on assignment.ClassSectionId equals section.Id
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where assignment.TeacherId == teacherId && assignment.IsActive && !assignment.IsDeleted
            select new { id = stage.Id, name = stage.NameAr }
        ).Distinct().OrderBy(x => x.name).ToListAsync(cancellationToken);

        if (stages.Count == 0)
        {
            stages = await _db.AcademicStages.AsNoTracking()
                .Where(x => x.SchoolId == teacher.SchoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new { id = x.Id, name = x.NameAr })
                .ToListAsync(cancellationToken);
        }

        return Json(stages);
    }

    [HttpGet]
    public async Task<IActionResult> Sections(int teacherId, int stageId, CancellationToken cancellationToken)
    {
        var assigned = await (
            from assignment in _db.TeacherAssignments.AsNoTracking()
            join section in _db.ClassSections.AsNoTracking() on assignment.ClassSectionId equals section.Id
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where assignment.TeacherId == teacherId
                  && assignment.IsActive
                  && !assignment.IsDeleted
                  && grade.AcademicStageId == stageId
                  && !section.IsDeleted
            orderby grade.SortOrder, section.NameAr
            select new { id = section.Id, name = grade.NameAr + " / " + section.NameAr }
        ).Distinct().ToListAsync(cancellationToken);

        if (assigned.Count > 0)
        {
            return Json(assigned);
        }

        var teacher = await _db.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == teacherId && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return Json(Array.Empty<object>());
        }

        var all = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.SchoolId == teacher.SchoolId
                  && grade.AcademicStageId == stageId
                  && !section.IsDeleted
                  && section.IsActive
            orderby grade.SortOrder, section.NameAr
            select new { id = section.Id, name = grade.NameAr + " / " + section.NameAr }
        ).ToListAsync(cancellationToken);

        return Json(all);
    }

    [HttpGet]
    public async Task<IActionResult> Roster(int teacherId, int stageId, int classSectionId, DateOnly? date, CancellationToken cancellationToken)
    {
        var rows = await _attendanceService.GetRosterAsync(new AttendanceRosterRequest
        {
            TeacherId = teacherId,
            AcademicStageId = stageId,
            ClassSectionId = classSectionId,
            AttendanceDate = date
        }, cancellationToken);
        return Json(rows);
    }

    private async Task LoadTeachersAsync(CancellationToken cancellationToken)
    {
        ViewBag.Teachers = new SelectList(
            await _db.Teachers.AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.FullNameAr)
                .Select(x => new { x.Id, Name = x.FullNameAr })
                .ToListAsync(cancellationToken),
            "Id", "Name");
    }
}
