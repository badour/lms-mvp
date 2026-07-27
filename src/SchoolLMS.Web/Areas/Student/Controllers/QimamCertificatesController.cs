using Microsoft.AspNetCore.Mvc;
using SchoolLMS.Application.Services.Students;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class QimamCertificatesController : StudentSectionController
{
    private readonly IQimamCertificateService _certificateService;

    public QimamCertificatesController(IQimamCertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "شهادات تطبيق قمم";
        var items = await _certificateService.GetForCurrentStudentAsync(cancellationToken);
        return View(items);
    }
}
