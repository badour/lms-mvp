using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Teachers;
using SchoolLMS.Application.Services.Teachers;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class TeachersController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly ApplicationDbContext _db;

    public TeachersController(ITeacherService teacherService, ApplicationDbContext db)
    {
        _teacherService = teacherService;
        _db = db;
    }

    public async Task<IActionResult> Index(TeacherSearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "المعلمون";
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", request.SchoolId);
        var result = await _teacherService.SearchAsync(request, cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.GetByIdAsync(id, cancellationToken);
        if (teacher is null)
        {
            TempData["Error"] = "المعلم غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "عرض المعلم";
        return View(teacher);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? schoolId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة معلم";
        var form = new TeacherForm
        {
            SchoolId = schoolId ?? 0,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            Gender = Gender.Male,
            MaritalStatus = MaritalStatus.Single,
            RoleName = "معلم",
            CreateLoginAccount = true,
            Password = "Teacher@12345"
        };
        await LoadFormLookupsAsync(form, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public async Task<IActionResult> Create(TeacherForm form, CancellationToken cancellationToken)
    {
        await using var attachment = ToUpload(form.Attachment);
        var request = ToRequest(form, attachment);
        var result = await _teacherService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadFormLookupsAsync(form, cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم إضافة المعلم بنجاح.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.GetByIdAsync(id, cancellationToken);
        if (teacher is null)
        {
            TempData["Error"] = "المعلم غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        var form = new TeacherForm
        {
            Id = teacher.Id,
            SchoolId = teacher.SchoolId,
            FullNameAr = teacher.FullNameAr,
            FullNameEn = teacher.FullNameEn,
            DocumentId = teacher.DocumentId ?? string.Empty,
            ParentName = teacher.ParentName ?? string.Empty,
            MotherName = teacher.MotherName ?? string.Empty,
            Gender = teacher.Gender,
            MaritalStatus = teacher.MaritalStatus,
            RoleName = teacher.RoleName,
            Phone = teacher.Phone ?? string.Empty,
            Email = teacher.Email,
            City = teacher.City ?? string.Empty,
            Address = teacher.Address ?? string.Empty,
            EducationalInfo = teacher.EducationalInfo,
            Specialization = teacher.Specialization,
            StartDate = teacher.StartDate,
            EndDate = teacher.EndDate,
            IsActive = teacher.IsActive,
            ClassSectionIds = teacher.Classes.Select(x => x.ClassSectionId).ToList(),
            OnlineLessonIds = teacher.OnlineLessons.Select(x => x.Id).ToList(),
            ExistingAttachmentName = teacher.AttachmentOriginalName,
            CreateLoginAccount = false,
            SubjectId = teacher.Classes.FirstOrDefault()?.SubjectId
        };

        ViewData["Title"] = "تعديل المعلم";
        await LoadFormLookupsAsync(form, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public async Task<IActionResult> Edit(int id, TeacherForm form, CancellationToken cancellationToken)
    {
        form.Id = id;
        await using var attachment = ToUpload(form.Attachment);
        var request = ToRequest(form, attachment);
        var result = await _teacherService.UpdateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadFormLookupsAsync(form, cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم تحديث بيانات المعلم.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _teacherService.DeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف المعلم."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Lookups(int schoolId, CancellationToken cancellationToken)
    {
        if (schoolId <= 0)
        {
            return Json(new { sections = Array.Empty<object>(), lessons = Array.Empty<object>(), subjects = Array.Empty<object>(), years = Array.Empty<object>() });
        }

        var sections = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where section.SchoolId == schoolId && !section.IsDeleted && section.IsActive
            orderby stage.SortOrder, grade.SortOrder, section.NameAr
            select new
            {
                id = section.Id,
                name = $"{stage.NameAr} / {grade.NameAr} / {section.NameAr}"
            }
        ).ToListAsync(cancellationToken);

        var lessons = await (
            from lesson in _db.Lessons.AsNoTracking()
            join subject in _db.Subjects.AsNoTracking() on lesson.SubjectId equals subject.Id
            where lesson.SchoolId == schoolId && !lesson.IsDeleted
            orderby lesson.TitleAr
            select new
            {
                id = lesson.Id,
                name = $"{lesson.TitleAr} ({subject.NameAr})",
                hasVideo = lesson.VideoPath != null || lesson.VideoUrl != null,
                materials = _db.LessonResources.Count(r => r.LessonId == lesson.Id && !r.IsDeleted)
            }
        ).ToListAsync(cancellationToken);

        var subjects = await _db.Subjects.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .OrderBy(x => x.NameAr)
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToListAsync(cancellationToken);

        var years = await _db.AcademicYears.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.Id)
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToListAsync(cancellationToken);

        return Json(new { sections, lessons, subjects, years });
    }

    private async Task LoadFormLookupsAsync(TeacherForm form, CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", form.SchoolId);

        if (form.SchoolId <= 0)
        {
            ViewBag.Sections = new MultiSelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Lessons = new MultiSelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Subjects = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Years = new SelectList(Enumerable.Empty<SelectListItem>());
            return;
        }

        var sections = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where section.SchoolId == form.SchoolId && !section.IsDeleted
            orderby stage.SortOrder, grade.SortOrder, section.NameAr
            select new { section.Id, Name = stage.NameAr + " / " + grade.NameAr + " / " + section.NameAr }
        ).ToListAsync(cancellationToken);
        ViewBag.Sections = new MultiSelectList(sections, "Id", "Name", form.ClassSectionIds);

        var lessons = await (
            from lesson in _db.Lessons.AsNoTracking()
            join subject in _db.Subjects.AsNoTracking() on lesson.SubjectId equals subject.Id
            where lesson.SchoolId == form.SchoolId && !lesson.IsDeleted
            orderby lesson.TitleAr
            select new { lesson.Id, Name = lesson.TitleAr + " (" + subject.NameAr + ")" }
        ).ToListAsync(cancellationToken);
        ViewBag.Lessons = new MultiSelectList(lessons, "Id", "Name", form.OnlineLessonIds);

        ViewBag.Subjects = new SelectList(
            await _db.Subjects.AsNoTracking().Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", form.SubjectId);

        ViewBag.Years = new SelectList(
            await _db.AcademicYears.AsNoTracking().Where(x => x.SchoolId == form.SchoolId && !x.IsDeleted).OrderByDescending(x => x.IsCurrent).ToListAsync(cancellationToken),
            "Id", "NameAr", form.AcademicYearId);
    }

    private static TeacherUpsertRequest ToRequest(TeacherForm form, FileUploadInput? attachment) => new()
    {
        Id = form.Id,
        SchoolId = form.SchoolId,
        FullNameAr = form.FullNameAr,
        FullNameEn = form.FullNameEn,
        DocumentId = form.DocumentId,
        ParentName = form.ParentName,
        MotherName = form.MotherName,
        Gender = form.Gender,
        MaritalStatus = form.MaritalStatus,
        RoleName = form.RoleName,
        Phone = form.Phone,
        Email = form.Email,
        City = form.City,
        Address = form.Address,
        EducationalInfo = form.EducationalInfo,
        Specialization = form.Specialization,
        StartDate = form.StartDate,
        EndDate = form.EndDate,
        IsActive = form.IsActive,
        SubjectId = form.SubjectId,
        AcademicYearId = form.AcademicYearId,
        ClassSectionIds = form.ClassSectionIds ?? [],
        OnlineLessonIds = form.OnlineLessonIds ?? [],
        Attachment = attachment,
        RemoveAttachment = form.RemoveAttachment,
        CreateLoginAccount = form.CreateLoginAccount,
        UserName = form.UserName,
        Password = form.Password
    };

    private static FileUploadInput? ToUpload(IFormFile? file)
    {
        if (file is null || file.Length <= 0)
        {
            return null;
        }

        return new FileUploadInput
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length,
            Content = file.OpenReadStream()
        };
    }
}
