using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student,SuperAdministrator,SchoolAdministrator,Parent,Teacher")]
public abstract class StudentSectionController : Controller
{
    protected IActionResult Section(string title, string description, string? eyebrow = null)
    {
        ViewData["Title"] = title;
        ViewData["Description"] = description;
        ViewData["Eyebrow"] = eyebrow ?? "بوابة الطالب";
        return View("~/Areas/Student/Views/Shared/Section.cshtml");
    }
}
