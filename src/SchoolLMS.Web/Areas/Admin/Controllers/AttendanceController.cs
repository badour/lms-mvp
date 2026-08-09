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

    public async Task<IActionResult> Index(int? schoolId, int? teacherId, DateOnly? attendanceDate, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "حضور الصفوف";
        await LoadFilterLookupsAsync(schoolId, teacherId, cancellationToken);

        var items = await _attendanceService.ListAsync(new AttendanceSessionFilter
        {
            SchoolId = schoolId,
            TeacherId = teacherId,
            AttendanceDate = attendanceDate
        }, cancellationToken);

        ViewBag.AttendanceDate = attendanceDate?.ToString("yyyy-MM-dd");
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "تسجيل حضور صف";
        await LoadSchoolsAsync(cancellationToken);
        ClearLookups();
        return View(new AttendanceCreateForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AttendanceCreateForm form, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.SaveAsync(ToRequest(form), cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadSchoolsAsync(cancellationToken);
            await LoadSchoolLookupsAsync(form, cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم حفظ حضور الصف بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var detail = await _attendanceService.GetByIdAsync(id, cancellationToken);
        if (detail is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "عرض سجل الحضور";
        return View(detail);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var detail = await _attendanceService.GetByIdAsync(id, cancellationToken);
        if (detail is null)
        {
            return NotFound();
        }

        var form = new AttendanceCreateForm
        {
            SessionId = detail.Id,
            SchoolId = detail.SchoolId,
            TeacherId = detail.TeacherId,
            AcademicStageId = detail.AcademicStageId,
            ClassSectionId = detail.ClassSectionId,
            AttendanceDate = detail.AttendanceDate,
            Students = detail.Students.Select(x => new AttendanceMarkInput
            {
                StudentId = x.StudentId,
                StudentNumber = x.StudentNumber,
                FullNameAr = x.FullNameAr,
                IsPresent = x.IsPresent
            }).ToList()
        };

        ViewData["Title"] = "تعديل سجل الحضور";
        await LoadSchoolsAsync(cancellationToken);
        await LoadSchoolLookupsAsync(form, cancellationToken);
        return View("Edit", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AttendanceCreateForm form, CancellationToken cancellationToken)
    {
        if (!form.SessionId.HasValue)
        {
            return BadRequest();
        }

        var result = await _attendanceService.SaveAsync(ToRequest(form), cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadSchoolsAsync(cancellationToken);
            await LoadSchoolLookupsAsync(form, cancellationToken);
            ViewData["Title"] = "تعديل سجل الحضور";
            return View(form);
        }

        TempData["Success"] = "تم تحديث سجل الحضور بنجاح.";
        return RedirectToAction(nameof(Details), new { id = form.SessionId.Value });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _attendanceService.DeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف سجل الحضور."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Teachers(int schoolId, CancellationToken cancellationToken)
    {
        var teachers = await _db.Teachers.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.FullNameAr)
            .Select(x => new { id = x.Id, name = x.FullNameAr })
            .ToListAsync(cancellationToken);
        return Json(teachers);
    }

    [HttpGet]
    public async Task<IActionResult> Stages(int schoolId, CancellationToken cancellationToken)
    {
        var stages = await _db.AcademicStages.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToListAsync(cancellationToken);
        return Json(stages);
    }

    [HttpGet]
    public async Task<IActionResult> Sections(int schoolId, int stageId, CancellationToken cancellationToken)
    {
        var sections = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.SchoolId == schoolId
                  && grade.AcademicStageId == stageId
                  && !section.IsDeleted
                  && section.IsActive
                  && grade.IsActive
                  && !grade.IsDeleted
            orderby grade.SortOrder, section.NameAr
            select new { id = section.Id, name = grade.NameAr + " / " + section.NameAr }
        ).ToListAsync(cancellationToken);
        return Json(sections);
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

    private static SaveAttendanceRequest ToRequest(AttendanceCreateForm form) => new()
    {
        SessionId = form.SessionId,
        SchoolId = form.SchoolId,
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

    private async Task LoadSchoolsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr");
    }

    private void ClearLookups()
    {
        ViewBag.Teachers = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
        ViewBag.Stages = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
        ViewBag.Sections = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
    }

    private async Task LoadSchoolLookupsAsync(AttendanceCreateForm form, CancellationToken cancellationToken)
    {
        if (form.SchoolId <= 0)
        {
            ClearLookups();
            return;
        }

        ViewBag.Teachers = new SelectList(
            await _db.Teachers.AsNoTracking()
                .Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.FullNameAr)
                .Select(x => new { x.Id, Name = x.FullNameAr })
                .ToListAsync(cancellationToken),
            "Id", "Name", form.TeacherId);

        ViewBag.Stages = new SelectList(
            await _db.AcademicStages.AsNoTracking()
                .Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new { x.Id, Name = x.NameAr })
                .ToListAsync(cancellationToken),
            "Id", "Name", form.AcademicStageId);

        if (form.AcademicStageId > 0)
        {
            var sections = await (
                from section in _db.ClassSections.AsNoTracking()
                join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
                where section.SchoolId == form.SchoolId
                      && grade.AcademicStageId == form.AcademicStageId
                      && !section.IsDeleted
                      && section.IsActive
                orderby grade.SortOrder, section.NameAr
                select new { section.Id, Name = grade.NameAr + " / " + section.NameAr }
            ).ToListAsync(cancellationToken);
            ViewBag.Sections = new SelectList(sections, "Id", "Name", form.ClassSectionId);
        }
        else
        {
            ViewBag.Sections = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
        }
    }

    private async Task LoadFilterLookupsAsync(int? schoolId, int? teacherId, CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);

        var teachersQuery = _db.Teachers.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        if (schoolId.HasValue)
        {
            teachersQuery = teachersQuery.Where(x => x.SchoolId == schoolId.Value);
        }

        ViewBag.Teachers = new SelectList(
            await teachersQuery.OrderBy(x => x.FullNameAr)
                .Select(x => new { x.Id, Name = x.FullNameAr })
                .ToListAsync(cancellationToken),
            "Id", "Name", teacherId);
    }
}
