using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolLMS.Application.Authorization;
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
        ViewData["Title"] = "المدارس";
        var items = await _schoolService.GetAllAsync(cancellationToken);
        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "إضافة مدرسة";
        return View(new CreateSchoolRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSchoolRequest request, CancellationToken cancellationToken)
    {
        var result = await _schoolService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(request);
        }

        TempData["Success"] = "تم إنشاء المدرسة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var school = await _schoolService.GetByIdAsync(id, cancellationToken);
        if (school is null) return NotFound();

        ViewData["Title"] = "تعديل مدرسة";
        return View(new UpdateSchoolRequest
        {
            Id = school.Id,
            NameAr = school.NameAr,
            NameEn = school.NameEn,
            Address = school.Address,
            Phone = school.Phone,
            Email = school.Email,
            SchoolType = school.SchoolType,
            GenderType = school.GenderType,
            Currency = school.Currency,
            IsActive = school.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSchoolRequest request, CancellationToken cancellationToken)
    {
        var result = await _schoolService.UpdateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(request);
        }

        TempData["Success"] = "تم تحديث المدرسة.";
        return RedirectToAction(nameof(Index));
    }
}
