using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ApplicationDbContext _db;

    public StudentsController(IStudentService studentService, ApplicationDbContext db)
    {
        _studentService = studentService;
        _db = db;
    }

    public async Task<IActionResult> Index(StudentSearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الطلاب";
        var result = await _studentService.SearchAsync(request, cancellationToken);
        ViewBag.Schools = new SelectList(await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken), "Id", "NameAr", request.SchoolId);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة طالب جديد";
        await LoadLookupsAsync(cancellationToken);
        return View(new CreateStudentRequest
        {
            AdmissionDate = DateOnly.FromDateTime(DateTime.Today),
            Gender = SchoolLMS.Domain.Enums.Gender.Male,
            Hobbies = [],
            NotesList = []
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var result = await _studentService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadLookupsAsync(cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم تسجيل الطالب بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookupsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken), "Id", "NameAr");
        ViewBag.Years = new SelectList(await _db.AcademicYears.AsNoTracking().Where(x => !x.IsDeleted && x.IsCurrent).ToListAsync(cancellationToken), "Id", "NameAr");
        ViewBag.Grades = new SelectList(await _db.GradeLevels.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken), "Id", "NameAr");
        ViewBag.Sections = new SelectList(await _db.ClassSections.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken), "Id", "NameAr");
    }
}
