using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class ProfileController : StudentSectionController
{
    public IActionResult Index() => Section("ملف الطالب", "البيانات الشخصية والأكاديمية ومعلومات التواصل.");
}
