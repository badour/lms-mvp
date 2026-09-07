using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Domain.Enums;
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
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            TempData["Error"] = "الطالب غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "عرض الطالب";
        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة طالب جديد";
        await LoadLookupsAsync(null, cancellationToken);
        return View(new CreateStudentRequest
        {
            AdmissionDate = DateOnly.FromDateTime(DateTime.Today),
            Gender = Gender.Male,
            Hobbies = [],
            NotesList = []
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        request.Hobbies ??= [];
        request.NotesList ??= [];
        var result = await _studentService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadLookupsAsync(request.SchoolId, cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم تسجيل الطالب بنجاح.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            TempData["Error"] = "الطالب غير موجود.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "تعديل طالب";
        await LoadLookupsAsync(student.SchoolId, cancellationToken);
        return View(new UpdateStudentRequest
        {
            Id = student.Id,
            SchoolId = student.SchoolId,
            SchoolBranchId = student.SchoolBranchId,
            StudentNumber = student.StudentNumber,
            FullNameAr = student.FullNameAr,
            FullNameEn = student.FullNameEn,
            FatherName = student.FatherName ?? string.Empty,
            MotherName = student.MotherName ?? string.Empty,
            PassportOrCardId = student.PassportOrCardId ?? student.NationalId ?? string.Empty,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            AdmissionDate = student.AdmissionDate,
            AcademicYearId = student.AcademicYearId,
            GradeLevelId = student.GradeLevelId,
            ClassSectionId = student.ClassSectionId,
            ClassClassification = student.ClassClassification,
            Phone1 = student.Phone1 ?? string.Empty,
            Phone2 = student.Phone2,
            City = student.City ?? string.Empty,
            Region = student.Region ?? string.Empty,
            Address = student.Address ?? string.Empty,
            Notes = student.Notes,
            EmergencyContactName = student.EmergencyContactName,
            EmergencyContactPhone = student.EmergencyContactPhone,
            BloodType = student.BloodType ?? string.Empty,
            DiseaseHistory = student.DiseaseHistory,
            PreviousSchool = student.PreviousSchool,
            PlaceOfBirth = student.PlaceOfBirth,
            Nationality = student.Nationality,
            Status = student.Status,
            Hobbies = student.Hobbies,
            NotesList = student.NotesList
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        request.Hobbies ??= [];
        request.NotesList ??= [];
        var result = await _studentService.UpdateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await LoadLookupsAsync(request.SchoolId, cancellationToken);
            return View(request);
        }

        TempData["Success"] = "تم تحديث بيانات الطالب.";
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _studentService.DeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "تم حذف الطالب."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookupsAsync(int? schoolId, CancellationToken cancellationToken)
    {
        ViewBag.Schools = new SelectList(
            await _db.Schools.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.NameAr).ToListAsync(cancellationToken),
            "Id", "NameAr", schoolId);

        var yearsQuery = _db.AcademicYears.AsNoTracking().Where(x => !x.IsDeleted && x.IsCurrent);
        var gradesQuery = _db.GradeLevels.AsNoTracking().Where(x => !x.IsDeleted);
        var sectionsQuery = _db.ClassSections.AsNoTracking().Where(x => !x.IsDeleted);

        if (schoolId is > 0)
        {
            yearsQuery = yearsQuery.Where(x => x.SchoolId == schoolId.Value);
            gradesQuery = gradesQuery.Where(x => x.SchoolId == schoolId.Value);
            sectionsQuery = sectionsQuery.Where(x => x.SchoolId == schoolId.Value);
        }

        ViewBag.Years = new SelectList(await yearsQuery.OrderByDescending(x => x.Id).ToListAsync(cancellationToken), "Id", "NameAr");
        ViewBag.Grades = new SelectList(await gradesQuery.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr).ToListAsync(cancellationToken), "Id", "NameAr");
        ViewBag.Sections = new SelectList(
            await (
                from section in sectionsQuery
                join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
                orderby grade.SortOrder, section.NameAr
                select new { section.Id, Name = grade.NameAr + " / " + section.NameAr }
            ).ToListAsync(cancellationToken),
            "Id", "Name");
    }
}
