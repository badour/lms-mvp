using Microsoft.AspNetCore.Mvc;
using SchoolLMS.Application.Services.Exams;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class ExamsController : StudentSectionController
{
    private readonly IExamAdminService _examService;

    public ExamsController(IExamAdminService examService)
    {
        _examService = examService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الامتحانات";
        var items = await _examService.GetForCurrentStudentAsync(cancellationToken);
        return View(items);
    }
}
