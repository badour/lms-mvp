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
        await LoadLookupsAsync(request.SchoolId, request.StudentId, cancellationToken);
        var result = await _certificateService.SearchAsync(request, cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? studentId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة شهادة قمم";
        await LoadLookupsAsync(null, studentId, cancellationToken);
        return View(new CreateQimamCertificateForm
        {
            StudentId = studentId ?? 0,
            CertificateDate = DateOnly.FromDateTime(DateTime.Today)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(15 * 1024 * 1024)]
    public async Task<IActionResult> Create(CreateQimamCertificateForm form, CancellationToken cancellationToken)
    {
        await using var image = ToUpload(form.Image);
        await using var document = ToUpload(form.Document);

        var request = new CreateQimamCertificateRequest
        {
            StudentId = form.StudentId,
            CertificateName = form.CertificateName,
            CertificateDate = form.CertificateDate,
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

            await LoadLookupsAsync(null, form.StudentId, cancellationToken);
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

    private async Task LoadLookupsAsync(int? schoolId, int? studentId, CancellationToken cancellationToken)
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
