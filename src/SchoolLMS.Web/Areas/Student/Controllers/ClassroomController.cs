using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class ClassroomController : StudentSectionController
{
    public IActionResult Index() => Section("القاعة الإلكترونية", "الدخول إلى الدروس الإلكترونية والموارد والأنشطة.");
}
