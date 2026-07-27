using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class HomeworkController : StudentSectionController
{
    public IActionResult Index() => Section("الواجبات البيتية", "عرض الواجبات المطلوبة والمسلّمة ومتابعة مواعيد التسليم والملاحظات.");
}
