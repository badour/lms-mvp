using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class BadgesController : StudentSectionController
{
    public IActionResult Index() => Section("الأوسمة والإنجازات", "عرض الأوسمة والنقاط والإنجازات المحققة.");
}
