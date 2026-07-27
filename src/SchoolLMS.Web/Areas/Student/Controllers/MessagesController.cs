using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class MessagesController : StudentSectionController
{
    public IActionResult Index() => Section("الطلبات والشكاوى", "إرسال الطلبات والشكاوى ومتابعة الردود والرسائل.");
}
