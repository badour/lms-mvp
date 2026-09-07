using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class QimamCertificatesController : Controller
{
    private readonly IQimamCertificateService _certificateService;
    private readonly ApplicationDbContext _db;

    public QimamCertificatesController(IQimamCertificateService certificateService, ApplicationDbContext db)
    {
        _certificateService = certificateService;
        _db = db;
    }

    public async Task<IActionResult> Index(QimamCertificateSearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "شهادات تطبيق قمم";
        await LoadLookupsAsync(request.SchoolId, request.StudentId, null, null, cancellationToken);
        var result = await _certificateService.SearchAsync(request, cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? studentId, int? schoolId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة شهادة قمم";

        var resolvedSchoolId = schoolId;
        if (studentId is > 0)
        {
            resolvedSchoolId ??= await _db.Students.AsNoTracking()
                .Where(x => x.Id == studentId.Value && !x.IsDeleted)
                .Select(x => (int?)x.SchoolId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var form = new CreateQimamCertificateForm
        {
            SchoolId = resolvedSchoolId ?? 0,
            StudentId = studentId ?? 0,
            CertificateDate = DateOnly.FromDateTime(DateTime.Today)
        };

        await LoadLookupsAsync(
            form.SchoolId > 0 ? form.SchoolId : null,
            form.StudentId > 0 ? form.StudentId : null,
            form.GradeLevelId,
            form.ClassSectionId,
            cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(15 * 1024 * 1024)]
    public async Task<IActionResult> Create(CreateQimamCertificateForm form, CancellationToken cancellationToken)
    {
        if (form.SchoolId <= 0)
        {
            ModelState.AddModelError(nameof(form.SchoolId), "اختر اسم المدرسة.");
        }
        else if (form.StudentId > 0)
        {
            var studentSchoolId = await _db.Students.AsNoTracking()
                .Where(x => x.Id == form.StudentId && !x.IsDeleted)
                .Select(x => (int?)x.SchoolId)
                .FirstOrDefaultAsync(cancellationToken);

            if (studentSchoolId is null)
            {
                ModelState.AddModelError(nameof(form.StudentId), "الطالب غير موجود.");
            }
            else if (studentSchoolId.Value != form.SchoolId)
            {
                ModelState.AddModelError(nameof(form.StudentId), "الطالب لا ينتمي للمدرسة المختارة.");
            }
        }

        if (form.SchoolId > 0 && form.GradeLevelId > 0)
        {
            var gradeBelongs = await _db.GradeLevels.AsNoTracking().AnyAsync(
                x => x.Id == form.GradeLevelId
                     && x.SchoolId == form.SchoolId
                     && !x.IsDeleted
                     && x.IsActive,
                cancellationToken);
            if (!gradeBelongs)
            {
                ModelState.AddModelError(nameof(form.GradeLevelId), "الصف المختار غير مرتبط بالمدرسة.");
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(
                form.SchoolId > 0 ? form.SchoolId : null,
                form.StudentId > 0 ? form.StudentId : null,
                form.GradeLevelId,
                form.ClassSectionId,
                cancellationToken);
            return View(form);
        }

        await using var image = ToUpload(form.Image);
        await using var document = ToUpload(form.Document);

        var request = new CreateQimamCertificateRequest
        {
            StudentId = form.StudentId,
            CertificateName = form.CertificateName,
            CertificateDate = form.CertificateDate,
            GradeLevelId = form.GradeLevelId,
            ClassSectionId = form.ClassSectionId,
            ClassName = form.ClassName,
            Notes = form.Notes,
            Description = form.Description,
            Image = image,
            Document = document
        };

        var result = await _certificateService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadLookupsAsync(
                form.SchoolId > 0 ? form.SchoolId : null,
                form.StudentId > 0 ? form.StudentId : null,
                form.GradeLevelId,
                form.ClassSectionId,
                cancellationToken);
            return View(form);
        }

        TempData["Success"] = "تم حفظ شهادة قمم بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _certificateService.DeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف الشهادة."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> StudentsBySchool(int schoolId, CancellationToken cancellationToken)
    {
        if (schoolId <= 0)
        {
            return Json(Array.Empty<object>());
        }

        var students = await _db.Students.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .OrderBy(x => x.FullNameAr)
            .Select(x => new { id = x.Id, name = x.FullNameAr + " (" + x.StudentNumber + ")" })
            .ToListAsync(cancellationToken);

        return Json(students);
    }

    [HttpGet]
    public async Task<IActionResult> GradesBySchool(int schoolId, CancellationToken cancellationToken)
    {
        if (schoolId <= 0)
        {
            return Json(Array.Empty<object>());
        }

        var grades = await (
            from grade in _db.GradeLevels.AsNoTracking()
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where grade.SchoolId == schoolId
                  && !grade.IsDeleted
                  && grade.IsActive
                  && !stage.IsDeleted
                  && stage.IsActive
            orderby stage.SortOrder, grade.SortOrder, grade.NameAr
            select new { id = grade.Id, name = grade.NameAr }
        ).ToListAsync(cancellationToken);

        return Json(grades);
    }

    [HttpGet]
    public async Task<IActionResult> GradesByStudent(int studentId, CancellationToken cancellationToken)
    {
        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted, cancellationToken);
        if (student is null)
        {
            return Json(Array.Empty<object>());
        }

        return await GradesBySchool(student.SchoolId, cancellationToken);
    }

    [HttpGet]
    public async Task<IActionResult> SectionsByGrade(int gradeLevelId, CancellationToken cancellationToken)
    {
        var sections = await _db.ClassSections.AsNoTracking()
            .Where(x => x.GradeLevelId == gradeLevelId && !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToListAsync(cancellationToken);

        return Json(sections);
    }

    private async Task LoadLookupsAsync(
        int? schoolId,
        int? studentId,
        int? gradeLevelId,
        int? classSectionId,
        CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);

        var studentsQuery = _db.Students.AsNoTracking().Where(x => !x.IsDeleted);
        if (schoolId.HasValue)
        {
            studentsQuery = studentsQuery.Where(x => x.SchoolId == schoolId.Value);
        }

        ViewBag.Students = new SelectList(
            await studentsQuery.OrderBy(x => x.FullNameAr).Select(x => new
            {
                x.Id,
                Name = x.FullNameAr + " (" + x.StudentNumber + ")"
            }).ToListAsync(cancellationToken),
            "Id", "Name", studentId);

        var gradeItems = new List<LookupItem>();
        var sectionItems = new List<LookupItem>();

        if (schoolId is > 0)
        {
            gradeItems = await (
                from grade in _db.GradeLevels.AsNoTracking()
                join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
                where grade.SchoolId == schoolId.Value
                      && !grade.IsDeleted
                      && grade.IsActive
                      && !stage.IsDeleted
                      && stage.IsActive
                orderby stage.SortOrder, grade.SortOrder, grade.NameAr
                select new LookupItem { Id = grade.Id, Name = grade.NameAr }
            ).ToListAsync(cancellationToken);
        }

        if (gradeLevelId is > 0)
        {
            sectionItems = await _db.ClassSections.AsNoTracking()
                .Where(x => x.GradeLevelId == gradeLevelId.Value && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new LookupItem { Id = x.Id, Name = x.NameAr })
                .ToListAsync(cancellationToken);
        }

        ViewBag.Grades = new SelectList(gradeItems, nameof(LookupItem.Id), nameof(LookupItem.Name), gradeLevelId);
        ViewBag.Sections = new SelectList(sectionItems, nameof(LookupItem.Id), nameof(LookupItem.Name), classSectionId);
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

    private sealed class LookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
