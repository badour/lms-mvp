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
        await LoadTeachersAsync(cancellationToken);
        return View(new ExamCreateForm
        {
            ExamDateTime = DateTime.Now.AddDays(1).Date.AddHours(9),
            Status = PublicationStatus.Draft
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamCreateForm form, CancellationToken cancellationToken)
    {
        var request = new CreateExamRequest
        {
            TeacherId = form.TeacherId,
            AcademicStageId = form.AcademicStageId,
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
            await LoadTeachersAsync(cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم إنشاء الامتحان بنجاح.";
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
