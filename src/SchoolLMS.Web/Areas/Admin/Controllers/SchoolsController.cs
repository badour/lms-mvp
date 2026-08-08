using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolLMS.Application.DTOs.Schools;
using SchoolLMS.Application.Services.Schools;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SchoolsController : Controller
{
    private readonly ISchoolService _schoolService;

    public SchoolsController(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "قائمة المدارس";
        var items = await _schoolService.GetAllAsync(cancellationToken);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var school = await _schoolService.GetByIdAsync(id, cancellationToken);
        if (school is null)
        {
            TempData["Error"] = "المدرسة غير موجودة.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "عرض المدرسة";
        return View(school);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة مدرسة جديدة";
        await LoadSchoolOptionsAsync(null, cancellationToken);
        return View(new CreateSchoolRequest
        {
            SchoolType = "Private",
            GenderType = "CoEducational",
            Currency = "IQD",
            YearName = $"{DateTime.Today.Year}-{DateTime.Today.Year + 1}",
            Stages =
            [
                new SchoolStageItemDto
                {
                    StageName = "المرحلة الابتدائية",
                    ClassName = "الأول ابتدائي",
                    SectionName = "أ",
                    YearName = $"{DateTime.Today.Year}-{DateTime.Today.Year + 1}",
                    IsActive = true
                }
            ]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSchoolRequest request, CancellationToken cancellationToken)
    {
        request.Stages ??= [];
        var result = await _schoolService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadSchoolOptionsAsync(null, cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم إنشاء المدرسة ومراحلها بنجاح.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var school = await _schoolService.GetByIdAsync(id, cancellationToken);
        if (school is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "تعديل مدرسة";
        await LoadSchoolOptionsAsync(id, cancellationToken);
        return View(new UpdateSchoolRequest
        {
            Id = school.Id,
            NameAr = school.NameAr,
            NameEn = school.NameEn,
            Address = school.Address ?? string.Empty,
            Phone = school.Phone,
            Email = school.Email ?? string.Empty,
            SchoolType = school.SchoolType,
            GenderType = school.GenderType,
            Currency = school.Currency,
            YearName = school.YearName ?? $"{DateTime.Today.Year}-{DateTime.Today.Year + 1}",
            IsActive = school.IsActive,
            Stages = school.Stages.Count > 0
                ? school.Stages.Select(s =>
                {
                    s.SchoolId = s.SchoolId > 0 ? s.SchoolId : school.Id;
                    return s;
                }).ToList()
                : [new SchoolStageItemDto { IsActive = true, YearName = school.YearName, SectionName = "أ", SchoolId = school.Id }]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSchoolRequest request, CancellationToken cancellationToken)
    {
        request.Stages ??= [];
        var result = await _schoolService.UpdateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadSchoolOptionsAsync(request.Id, cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم تحديث المدرسة والمراحل.";
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _schoolService.DeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف المدرسة وجميع مراحلها المرتبطة."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSchoolOptionsAsync(int? selectedSchoolId, CancellationToken cancellationToken)
    {
        var schools = await _schoolService.GetAllAsync(cancellationToken);
        ViewBag.SchoolOptions = new SelectList(schools, nameof(SchoolListItemDto.Id), nameof(SchoolListItemDto.NameAr), selectedSchoolId);
        ViewBag.SchoolOptionsJson = schools
            .Select(x => new { id = x.Id, name = x.NameAr })
            .ToList();
    }
}
