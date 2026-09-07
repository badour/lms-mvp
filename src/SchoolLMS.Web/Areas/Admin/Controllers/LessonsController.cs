using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Lessons;
using SchoolLMS.Application.Services.Lessons;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class LessonsController : Controller
{
    private readonly ILessonService _lessonService;
    private readonly ApplicationDbContext _db;

    public LessonsController(ILessonService lessonService, ApplicationDbContext db)
    {
        _lessonService = lessonService;
        _db = db;
    }

    public async Task<IActionResult> Index(LessonSearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الدروس الإلكترونية";
        await LoadFilterLookupsAsync(request.SchoolId, cancellationToken);
        var result = await _lessonService.SearchAsync(request, cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var lesson = await _lessonService.GetByIdAsync(id, cancellationToken);
        if (lesson is null)
        {
            TempData["Error"] = "الدرس غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "عرض الدرس";
        return View(lesson);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? schoolId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة درس جديد";
        var form = new CreateLessonForm
        {
            SchoolId = schoolId ?? 0,
            LessonDateTime = DateTime.Now.AddHours(1),
            Status = PublicationStatus.Draft,
            IsPosted = false
        };
        await LoadFormLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> Create(CreateLessonForm form, CancellationToken cancellationToken)
    {
        var uploads = new List<FileUploadInput>();
        FileUploadInput? video = null;

        try
        {
            video = ToUpload(form.Video);
            uploads.AddRange(ToUploads(form.Materials));

            var request = ToCreateRequest(form, video, uploads);
            var result = await _lessonService.CreateAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                await LoadFormLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, cancellationToken);
                return View(form);
            }

            TempData["Success"] = "تم حفظ الدرس الإلكتروني بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        finally
        {
            await DisposeUploadsAsync(video, uploads);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var lesson = await _lessonService.GetByIdAsync(id, cancellationToken);
        if (lesson is null)
        {
            TempData["Error"] = "الدرس غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "تعديل الدرس";
        var form = new CreateLessonForm
        {
            Id = lesson.Id,
            SchoolId = lesson.SchoolId,
            SubjectId = lesson.SubjectId,
            TeacherId = lesson.TeacherId,
            ClassSectionIds = lesson.ClassSectionIds,
            Description = lesson.Description,
            Notes = lesson.Notes,
            LessonDateTime = lesson.LessonDateTime,
            Status = lesson.Status,
            IsPosted = lesson.IsPosted,
            ExistingVideoName = lesson.VideoOriginalName,
            ExistingMaterials = lesson.Materials.Select(x => new ExistingLessonMaterialFormItem
            {
                Id = x.Id,
                Title = x.Title,
                OriginalFileName = x.OriginalFileName
            }).ToList()
        };

        await LoadFormLookupsAsync(form.SchoolId, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> Edit(int id, CreateLessonForm form, CancellationToken cancellationToken)
    {
        form.Id = id;
        var uploads = new List<FileUploadInput>();
        FileUploadInput? video = null;

        try
        {
            video = ToUpload(form.Video);
            uploads.AddRange(ToUploads(form.Materials));

            var request = new UpdateLessonRequest
            {
                Id = id,
                SchoolId = form.SchoolId,
                SubjectId = form.SubjectId,
                TeacherId = form.TeacherId,
                ClassSectionIds = (form.ClassSectionIds ?? []).Where(x => x > 0).Distinct().ToList(),
                Description = form.Description,
                Notes = form.Notes,
                LessonDateTime = form.LessonDateTime,
                Status = form.Status,
                IsPosted = form.IsPosted,
                RemoveVideo = form.RemoveVideo,
                RemoveMaterialIds = form.RemoveMaterialIds ?? [],
                Video = video,
                Materials = uploads
            };

            var result = await _lessonService.UpdateAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                var current = await _lessonService.GetByIdAsync(id, cancellationToken);
                form.ExistingVideoName = current?.VideoOriginalName;
                form.ExistingMaterials = current?.Materials.Select(x => new ExistingLessonMaterialFormItem
                {
                    Id = x.Id,
                    Title = x.Title,
                    OriginalFileName = x.OriginalFileName
                }).ToList() ?? [];

                await LoadFormLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, cancellationToken);
                ViewData["Title"] = "تعديل الدرس";
                return View(form);
            }

            TempData["Success"] = "تم تحديث الدرس بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        finally
        {
            await DisposeUploadsAsync(video, uploads);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _lessonService.DeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف الدرس مع الفيديو والمواد من النظام والملفات."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Lookups(int schoolId, CancellationToken cancellationToken)
    {
        if (schoolId <= 0)
        {
            return Json(new { subjects = Array.Empty<object>(), teachers = Array.Empty<object>(), classes = Array.Empty<object>() });
        }

        var subjects = await _db.Subjects.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToListAsync(cancellationToken);

        var teachers = await _db.Teachers.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.FullNameAr)
            .Select(x => new { id = x.Id, name = x.FullNameAr + (x.Specialization == null ? "" : " (" + x.Specialization + ")") })
            .ToListAsync(cancellationToken);

        var classes = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.SchoolId == schoolId && !section.IsDeleted && section.IsActive
            orderby grade.SortOrder, section.NameAr
            select new { id = section.Id, name = grade.NameAr + " / " + section.NameAr }
        ).ToListAsync(cancellationToken);

        return Json(new { subjects, teachers, classes });
    }

    private static CreateLessonRequest ToCreateRequest(CreateLessonForm form, FileUploadInput? video, List<FileUploadInput> materials) =>
        new()
        {
            SchoolId = form.SchoolId,
            SubjectId = form.SubjectId,
            TeacherId = form.TeacherId,
            ClassSectionIds = (form.ClassSectionIds ?? []).Where(x => x > 0).Distinct().ToList(),
            Description = form.Description,
            Notes = form.Notes,
            LessonDateTime = form.LessonDateTime,
            Status = form.Status,
            IsPosted = form.IsPosted,
            Video = video,
            Materials = materials
        };

    private async Task LoadFilterLookupsAsync(int? schoolId, CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);

        ViewBag.Statuses = new SelectList(new[]
        {
            new { Id = (int)PublicationStatus.Draft, Name = "مسودة" },
            new { Id = (int)PublicationStatus.Published, Name = "منشور" },
            new { Id = (int)PublicationStatus.Archived, Name = "مؤرشف" },
            new { Id = (int)PublicationStatus.Cancelled, Name = "ملغى" }
        }, "Id", "Name");
    }

    private async Task LoadFormLookupsAsync(int? schoolId, CancellationToken cancellationToken)
    {
        await LoadFilterLookupsAsync(schoolId, cancellationToken);

        if (!schoolId.HasValue || schoolId.Value <= 0)
        {
            ViewBag.Subjects = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Teachers = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.ClassOptions = Enumerable.Empty<SelectListItem>();
            return;
        }

        ViewBag.Subjects = new SelectList(
            await _db.Subjects.AsNoTracking()
                .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new { x.Id, Name = x.NameAr })
                .ToListAsync(cancellationToken),
            "Id", "Name");

        ViewBag.Teachers = new SelectList(
            await _db.Teachers.AsNoTracking()
                .Where(x => x.SchoolId == schoolId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.FullNameAr)
                .Select(x => new { x.Id, Name = x.FullNameAr })
                .ToListAsync(cancellationToken),
            "Id", "Name");

        ViewBag.ClassOptions = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.SchoolId == schoolId && !section.IsDeleted && section.IsActive
            orderby grade.SortOrder, section.NameAr
            select new SelectListItem
            {
                Value = section.Id.ToString(),
                Text = grade.NameAr + " / " + section.NameAr
            }
        ).ToListAsync(cancellationToken);
    }

    private static List<FileUploadInput> ToUploads(List<IFormFile>? files)
    {
        var list = new List<FileUploadInput>();
        if (files is null)
        {
            return list;
        }

        foreach (var file in files.Where(x => x is { Length: > 0 }))
        {
            var upload = ToUpload(file);
            if (upload is not null)
            {
                list.Add(upload);
            }
        }

        return list;
    }

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

    private static async Task DisposeUploadsAsync(FileUploadInput? video, List<FileUploadInput> uploads)
    {
        if (video is not null)
        {
            await video.DisposeAsync();
        }

        foreach (var upload in uploads)
        {
            await upload.DisposeAsync();
        }
    }
}
