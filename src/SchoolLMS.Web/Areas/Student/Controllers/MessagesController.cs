using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolLMS.Application.DTOs.Messages;
using SchoolLMS.Application.Services.Messages;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Web.Areas.Student.Controllers;

public class MessagesController : StudentSectionController
{
    private readonly IStudentInboxService _inboxService;

    public MessagesController(IStudentInboxService inboxService)
    {
        _inboxService = inboxService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "صندوق الرسائل";
        var messages = await _inboxService.GetMyMessagesAsync(cancellationToken);
        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> Compose(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إرسال رسالة";
        var instructors = await _inboxService.GetMyInstructorsAsync(cancellationToken);
        await LoadTargetLookupsAsync(instructors);
        return View(new SendStudentInboxMessageRequest
        {
            TargetType = StudentMessageTargetType.SchoolManagement
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Compose(SendStudentInboxMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await _inboxService.SendAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var instructors = await _inboxService.GetMyInstructorsAsync(cancellationToken);
            await LoadTargetLookupsAsync(instructors);
            ViewData["Title"] = "إرسال رسالة";
            return View(request);
        }

        TempData["Success"] = "تم إرسال الرسالة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var message = await _inboxService.GetByIdAsync(id, cancellationToken);
        if (message is null)
        {
            TempData["Error"] = "الرسالة غير موجودة.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "تفاصيل الرسالة";
        return View(message);
    }

    private Task LoadTargetLookupsAsync(IReadOnlyList<InstructorOptionDto> instructors)
    {
        ViewBag.Instructors = new SelectList(
            instructors.Select(x => new
            {
                x.TeacherId,
                Name = string.IsNullOrWhiteSpace(x.Specialization)
                    ? x.FullNameAr
                    : $"{x.FullNameAr} ({x.Specialization})"
            }),
            "TeacherId",
            "Name");

        ViewBag.TargetTypes = new SelectList(new[]
        {
            new { Id = (int)StudentMessageTargetType.SystemAdmin, Name = "إدارة النظام" },
            new { Id = (int)StudentMessageTargetType.SchoolManagement, Name = "إدارة المدرسة" },
            new { Id = (int)StudentMessageTargetType.Instructor, Name = "المعلم / المدرّس" }
        }, "Id", "Name");

        return Task.CompletedTask;
    }
}
