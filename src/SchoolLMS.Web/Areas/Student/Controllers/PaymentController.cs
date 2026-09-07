using Microsoft.AspNetCore.Mvc;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class PaymentController : StudentSectionController
{
    public IActionResult Index() => Section("الدفع الإلكتروني", "متابعة الرسوم والأقساط والمدفوعات الإلكترونية.");
}
