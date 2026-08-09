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
            ExamDateTime = DateTime.Today.AddDays(1).AddHours(9),
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
            SubjectId = form.SubjectId,
            TeacherId = form.TeacherId,
            ClassSectionId = form.ClassSectionId,
            ExamDateTime = form.ExamDateTime,
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

        TempData["Success"] = "تم إنشاء الامتحان بنجاح. سيظهر للطلاب والمعلم المرتبطين عند تفعيله (منشور).";
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
    public async Task<IActionResult> Sections(int schoolId, CancellationToken cancellationToken)
    {
        var sections = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where section.SchoolId == schoolId
                  && !section.IsDeleted
                  && section.IsActive
                  && grade.IsActive
                  && !grade.IsDeleted
                  && stage.IsActive
                  && !stage.IsDeleted
            orderby stage.SortOrder, grade.SortOrder, section.NameAr
            select new { id = section.Id, name = stage.NameAr + " / " + grade.NameAr + " / " + section.NameAr }
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

        var sections = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where section.SchoolId == form.SchoolId
                  && !section.IsDeleted
                  && section.IsActive
                  && grade.IsActive
                  && !grade.IsDeleted
                  && stage.IsActive
                  && !stage.IsDeleted
            orderby stage.SortOrder, grade.SortOrder, section.NameAr
            select new { section.Id, Name = stage.NameAr + " / " + grade.NameAr + " / " + section.NameAr }
        ).ToListAsync(cancellationToken);
        ViewBag.Sections = new SelectList(sections, "Id", "Name", form.ClassSectionId);
    }
}
