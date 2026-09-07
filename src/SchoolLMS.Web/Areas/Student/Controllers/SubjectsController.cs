using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class SubjectsController : StudentSectionController
{
    public IActionResult Index() => Section("المواد الدراسية", "استعراض المواد المسجلة والمعلمين والمحتوى المرتبط.");
}
