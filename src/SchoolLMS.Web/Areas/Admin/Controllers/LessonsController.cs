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
        await LoadFormLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, null, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(220 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 220 * 1024 * 1024)]
    public async Task<IActionResult> Create(CreateLessonForm form, CancellationToken cancellationToken)
    {
        var uploads = new List<FileUploadInput>();
        FileUploadInput? video = null;

        try
        {
            video = ToUpload(form.Video);
            if (form.Materials is not null)
            {
                foreach (var file in form.Materials.Where(x => x is { Length: > 0 }))
                {
                    var upload = ToUpload(file);
                    if (upload is not null)
                    {
                        uploads.Add(upload);
                    }
                }
            }

            var request = new CreateLessonRequest
            {
                SchoolId = form.SchoolId,
                SubjectId = form.SubjectId,
                TeacherId = form.TeacherId,
                ClassSectionIds = form.ClassSectionIds ?? [],
                Description = form.Description,
                Notes = form.Notes,
                LessonDateTime = form.LessonDateTime,
                Status = form.Status,
                IsPosted = form.IsPosted,
                Video = video,
                Materials = uploads
            };

            var result = await _lessonService.CreateAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                await LoadFormLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, form.ClassSectionIds, cancellationToken);
                return View(form);
            }

            TempData["Success"] = "تم حفظ الدرس الإلكتروني بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        finally
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

    private async Task LoadFormLookupsAsync(int? schoolId, IEnumerable<int>? selectedClassIds, CancellationToken cancellationToken)
    {
        await LoadFilterLookupsAsync(schoolId, cancellationToken);

        if (!schoolId.HasValue || schoolId.Value <= 0)
        {
            ViewBag.Subjects = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Teachers = new SelectList(Enumerable.Empty<object>(), "Id", "Name");
            ViewBag.Classes = new MultiSelectList(Enumerable.Empty<object>(), "Id", "Name");
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

        ViewBag.Classes = new MultiSelectList(
            await (
                from section in _db.ClassSections.AsNoTracking()
                join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
                where section.SchoolId == schoolId && !section.IsDeleted && section.IsActive
                orderby grade.SortOrder, section.NameAr
                select new { section.Id, Name = grade.NameAr + " / " + section.NameAr }
            ).ToListAsync(cancellationToken),
            "Id", "Name", selectedClassIds ?? Array.Empty<int>());
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
}
