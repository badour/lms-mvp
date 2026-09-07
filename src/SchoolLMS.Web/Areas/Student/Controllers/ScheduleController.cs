using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class ScheduleController : StudentSectionController
{
    public IActionResult Index() => Section("الجدول الأسبوعي", "عرض الجدول الدراسي اليومي والأسبوعي للحصص.");
}
