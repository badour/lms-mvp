using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class AttendanceController : StudentSectionController
{
    public IActionResult Index() => Section("الحضور والغياب", "عرض سجل الحضور والغياب والتأخير ونسبة الحضور.");
}
