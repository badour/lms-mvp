using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class ExamsController : StudentSectionController
{
    public IActionResult Index() => Section("الامتحانات", "متابعة الامتحانات القادمة والنتائج والجداول الزمنية.");
}
