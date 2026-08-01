using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Schedules;
using SchoolLMS.Application.Services.Schedules;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SchedulesController : Controller
{
    private readonly IScheduleAdminService _scheduleService;
    private readonly ApplicationDbContext _db;

    public SchedulesController(IScheduleAdminService scheduleService, ApplicationDbContext db)
    {
        _scheduleService = scheduleService;
        _db = db;
    }

    public async Task<IActionResult> Index(int? schoolId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الجداول الدراسية";
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);
        var items = await _scheduleService.ListAsync(schoolId, cancellationToken);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة جدول مدرسي";
        await LoadSchoolsAsync(cancellationToken);
        return View(new SaveScheduleRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveScheduleRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleService.SaveAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadSchoolsAsync(cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم حفظ الجدول الدراسي.";
        return RedirectToAction(nameof(Details), new { id = request.ClassSectionId });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleService.GetBySectionAsync(id, cancellationToken);
        if (schedule is null)
        {
            TempData["Error"] = "الجدول غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "عرض الجدول";
        ViewBag.DayNames = ScheduleAdminService.DayNamesAr;
        return View(schedule);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleService.GetBySectionAsync(id, cancellationToken);
        if (schedule is null)
        {
            TempData["Error"] = "الجدول غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "تعديل الجدول";
        await LoadSchoolsAsync(cancellationToken);
        ViewBag.DayNames = ScheduleAdminService.DayNamesAr;
        ViewBag.Schedule = schedule;
        return View(new SaveScheduleRequest
        {
            SchoolId = schedule.SchoolId,
            AcademicStageId = schedule.AcademicStageId,
            ClassSectionId = schedule.ClassSectionId,
            Cells = schedule.Cells
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaveScheduleRequest request, CancellationToken cancellationToken)
    {
        request.ClassSectionId = id;
        var result = await _scheduleService.SaveAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            var schedule = await _scheduleService.GetBySectionAsync(id, cancellationToken);
            ViewBag.Schedule = schedule;
            ViewBag.DayNames = ScheduleAdminService.DayNamesAr;
            await LoadSchoolsAsync(cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم تحديث الجدول.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _scheduleService.DeleteBySectionAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف الجدول."
            : result.Error;
        return RedirectToAction(nameof(Index));
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
    public async Task<IActionResult> GridData(int classSectionId, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleService.GetBySectionAsync(classSectionId, cancellationToken);
        if (schedule is null)
        {
            return Json(new { periods = Array.Empty<string>(), cells = Array.Empty<object>() });
        }

        return Json(new
        {
            periods = schedule.PeriodNames,
            cells = schedule.Cells.Select(c => new
            {
                day = c.DayOfWeek,
                period = c.PeriodSortOrder,
                text = c.EntryText,
                subjectId = c.SubjectId,
                teacherId = c.TeacherId
            })
        });
    }

    private async Task LoadSchoolsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr");
    }
}
