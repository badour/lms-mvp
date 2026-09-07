using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Messages;
using SchoolLMS.Application.Services.Messages;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Infrastructure.Persistence;
using SchoolLMS.Web.Models;

namespace SchoolLMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class MessagingController : Controller
{
    private readonly IAdminMessagingService _messagingService;
    private readonly ApplicationDbContext _db;

    public MessagingController(IAdminMessagingService messagingService, ApplicationDbContext db)
    {
        _messagingService = messagingService;
        _db = db;
    }

    public async Task<IActionResult> Index(AdminMessageSearchRequest request, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "التواصل والمراسلات";
        await LoadSchoolFilterAsync(request.SchoolId, cancellationToken);
        ViewBag.Sources = new SelectList(new[]
        {
            new { Value = "all", Text = "الكل" },
            new { Value = "student", Text = "طلاب" },
            new { Value = "instructor", Text = "معلمون" },
            new { Value = "management", Text = "إدارة المدارس" },
            new { Value = "admin", Text = "إدارة النظام" }
        }, "Value", "Text", request.Source ?? "all");

        var result = await _messagingService.SearchAsync(request, cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var message = await _messagingService.GetByIdAsync(id, cancellationToken);
        if (message is null)
        {
            TempData["Error"] = "الرسالة غير موجودة.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "عرض الرسالة";
        ViewBag.ReplyForm = new AdminReplyMessageForm { ParentMessageId = message.Id };
        return View(message);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? schoolId, StudentMessageTargetType? recipientKind, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "رسالة جديدة";
        var form = new AdminSendMessageForm
        {
            SchoolId = schoolId ?? 0,
            RecipientKind = recipientKind is StudentMessageTargetType.Student or StudentMessageTargetType.SchoolManagement
                ? recipientKind.Value
                : StudentMessageTargetType.Student
        };
        await LoadCreateLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, form.StudentId, form.ManagementUserId, cancellationToken);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public async Task<IActionResult> Create(AdminSendMessageForm form, CancellationToken cancellationToken)
    {
        var uploads = ToUploads(form.Attachments);
        try
        {
            // Resolve numeric الرقم (Students grid first column) when the posted value is not a PK.
            form.StudentId = await ResolveStudentIdAsync(form.SchoolId, form.StudentId, cancellationToken);

            var request = new AdminSendMessageRequest
            {
                SchoolId = form.SchoolId,
                RecipientKind = form.RecipientKind,
                StudentId = form.StudentId,
                ManagementUserId = form.ManagementUserId,
                Subject = form.Subject,
                Body = form.Body,
                Attachments = uploads
            };

            var result = await _messagingService.SendAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                await LoadCreateLookupsAsync(form.SchoolId > 0 ? form.SchoolId : null, form.StudentId, form.ManagementUserId, cancellationToken);
                return View(form);
            }

            TempData["Success"] = "تم إرسال الرسالة بنجاح.";
            return RedirectToAction(nameof(Details), new { id = result.Data });
        }
        finally
        {
            await DisposeUploadsAsync(uploads);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public async Task<IActionResult> Reply(AdminReplyMessageForm form, CancellationToken cancellationToken)
    {
        var uploads = ToUploads(form.Attachments);
        try
        {
            var request = new AdminReplyMessageRequest
            {
                ParentMessageId = form.ParentMessageId,
                Body = form.Body,
                Attachments = uploads
            };

            var result = await _messagingService.ReplyAsync(request, cancellationToken);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ", result.Errors);
                return RedirectToAction(nameof(Details), new { id = form.ParentMessageId });
            }

            TempData["Success"] = "تم إرسال الرد بنجاح.";
            return RedirectToAction(nameof(Details), new { id = form.ParentMessageId });
        }
        finally
        {
            await DisposeUploadsAsync(uploads);
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchStudents(int schoolId, string? term, CancellationToken cancellationToken)
    {
        var items = await _messagingService.SearchStudentsAsync(schoolId, term, cancellationToken);
        return Json(items);
    }

    [HttpGet]
    public async Task<IActionResult> ManagementAccounts(int schoolId, CancellationToken cancellationToken)
    {
        var items = await _messagingService.GetManagementAccountsAsync(schoolId, cancellationToken);
        return Json(items);
    }

    private async Task LoadSchoolFilterAsync(int? selectedSchoolId, CancellationToken cancellationToken)
    {
        var schools = await _db.Schools.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.NameAr,
                Selected = selectedSchoolId == x.Id
            })
            .ToListAsync(cancellationToken);

        ViewBag.Schools = schools;
    }

    private async Task LoadCreateLookupsAsync(
        int? schoolId,
        int? selectedStudentId,
        string? selectedManagementUserId,
        CancellationToken cancellationToken)
    {
        await LoadSchoolFilterAsync(schoolId, cancellationToken);
        ViewBag.RecipientKinds = new SelectList(new[]
        {
            new { Value = ((int)StudentMessageTargetType.Student).ToString(), Text = "طالب" },
            new { Value = ((int)StudentMessageTargetType.SchoolManagement).ToString(), Text = "إدارة المدرسة" }
        }, "Value", "Text");

        if (schoolId is > 0)
        {
            var students = await _messagingService.SearchStudentsAsync(schoolId.Value, null, cancellationToken);
            ViewBag.Students = new SelectList(
                students.Select(x => new
                {
                    x.Id,
                    // Match Students Index first column: الرقم ثم الاسم
                    Name = string.IsNullOrWhiteSpace(x.Extra) ? x.Name : $"{x.Extra} — {x.Name}"
                }),
                "Id",
                "Name",
                selectedStudentId?.ToString());

            ViewBag.ManagementAccounts = new SelectList(
                await _messagingService.GetManagementAccountsAsync(schoolId.Value, cancellationToken),
                "Id",
                "Name",
                selectedManagementUserId);
        }
        else
        {
            ViewBag.Students = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ManagementAccounts = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }

    /// <summary>
    /// Accepts either Students.Id or numeric StudentNumber (الرقم from the Students grid).
    /// </summary>
    private async Task<int?> ResolveStudentIdAsync(int schoolId, int? postedValue, CancellationToken cancellationToken)
    {
        if (schoolId <= 0 || !postedValue.HasValue || postedValue.Value <= 0)
        {
            return postedValue;
        }

        var byId = await _db.Students.AsNoTracking()
            .AnyAsync(x => x.Id == postedValue.Value && x.SchoolId == schoolId && !x.IsDeleted, cancellationToken);
        if (byId)
        {
            return postedValue;
        }

        var number = postedValue.Value.ToString();
        var byNumber = await _db.Students.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && x.StudentNumber == number && !x.IsDeleted)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return byNumber ?? postedValue;
    }

    private static List<FileUploadInput> ToUploads(List<IFormFile>? files)
    {
        var list = new List<FileUploadInput>();
        if (files is null)
        {
            return list;
        }

        foreach (var file in files.Where(x => x is { Length: > 0 }))
        {
            list.Add(new FileUploadInput
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length,
                Content = file.OpenReadStream()
            });
        }

        return list;
    }

    private static async Task DisposeUploadsAsync(List<FileUploadInput> uploads)
    {
        foreach (var upload in uploads)
        {
            await upload.DisposeAsync();
        }
    }
}
