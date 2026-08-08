using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Exams;
using SchoolLMS.Application.Services.Exams;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ExamsController : Controller
{
    private readonly IExamAdminService _examService;
    private readonly ApplicationDbContext _db;

    public ExamsController(IExamAdminService examService, ApplicationDbContext db)
    {
        _examService = examService;
        _db = db;
    }

    public async Task<IActionResult> Index(int? schoolId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الامتحانات";
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);
        var items = await _examService.ListRecentAsync(schoolId, cancellationToken);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إنشاء امتحان";
        var form = new ExamCreateForm
        {
            ExamDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Status = PublicationStatus.Published
        };
        await LoadSchoolsAsync(cancellationToken);
        await LoadSchoolLookupsAsync(form, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamCreateForm form, CancellationToken cancellationToken)
    {
        var request = new CreateExamRequest
        {
            SchoolId = form.SchoolId,
            TeachingPeriodId = form.TeachingPeriodId,
            SubjectId = form.SubjectId,
            TeacherId = form.TeacherId,
            AcademicStageId = form.AcademicStageId,
            ClassSectionId = form.ClassSectionId,
            ExamDate = form.ExamDate,
            Notes = form.Notes,
            Instructions = form.Instructions,
            Status = form.Status
        };

        var result = await _examService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadSchoolsAsync(cancellationToken);
            await LoadSchoolLookupsAsync(form, cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم إنشاء الامتحان بنجاح. سيظهر لطلاب المدرسة والمرحلة والصف المحددين.";
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
    public async Task<IActionResult> Subjects(int schoolId, CancellationToken cancellationToken)
    {
        var subjects = await _db.Subjects.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToListAsync(cancellationToken);
        return Json(subjects);
    }

    [HttpGet]
    public async Task<IActionResult> Periods(int schoolId, CancellationToken cancellationToken)
    {
        var periods = await _db.TeachingPeriods
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        if (periods.Count < 8)
        {
            var startHour = 8;
            for (var i = periods.Count + 1; i <= 8; i++)
            {
                var start = new TimeOnly(startHour + (i - 1), 0);
                var end = start.AddMinutes(45);
                var period = new SchoolLMS.Domain.Entities.Academic.TeachingPeriod
                {
                    SchoolId = schoolId,
                    NameAr = $"الحصة {i}",
                    NameEn = $"Period {i}",
                    StartTime = start,
                    EndTime = end,
                    SortOrder = i,
                    IsBreak = false
                };
                _db.TeachingPeriods.Add(period);
                periods.Add(period);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        return Json(periods.OrderBy(x => x.SortOrder).Take(8).Select(x => new
        {
            id = x.Id,
            name = $"{x.NameAr} ({x.StartTime:HH\\:mm} - {x.EndTime:HH\\:mm})"
        }));
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

    private async Task LoadSchoolsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr");
    }

    private async Task LoadSchoolLookupsAsync(ExamCreateForm form, CancellationToken cancellationToken)
    {
        if (form.SchoolId <= 0)
        {
            ViewBag.Teachers = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Subjects = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Periods = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Stages = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Sections = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            return;
        }

        ViewBag.Teachers = new SelectList(
            await _db.Teachers.AsNoTracking().Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.FullNameAr).Select(x => new { x.Id, Name = x.FullNameAr }).ToListAsync(cancellationToken),
            "Id", "Name", form.TeacherId);

        ViewBag.Subjects = new SelectList(
            await _db.Subjects.AsNoTracking().Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.NameAr).Select(x => new { x.Id, Name = x.NameAr }).ToListAsync(cancellationToken),
            "Id", "Name", form.SubjectId);

        var periods = await _db.TeachingPeriods.AsNoTracking()
            .Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .Take(8)
            .ToListAsync(cancellationToken);
        ViewBag.Periods = new SelectList(
            periods.Select(x => new { x.Id, Name = $"{x.NameAr} ({x.StartTime:HH\\:mm} - {x.EndTime:HH\\:mm})" }),
            "Id", "Name", form.TeachingPeriodId);

        ViewBag.Stages = new SelectList(
            await _db.AcademicStages.AsNoTracking().Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder).Select(x => new { x.Id, Name = x.NameAr }).ToListAsync(cancellationToken),
            "Id", "Name", form.AcademicStageId);

        if (form.AcademicStageId > 0)
        {
            var sections = await (
                from section in _db.ClassSections.AsNoTracking()
                join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
                where section.SchoolId == form.SchoolId
                      && grade.AcademicStageId == form.AcademicStageId
                      && !section.IsDeleted
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
}
