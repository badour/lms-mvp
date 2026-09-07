using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Web.Models;

public class AdminSendMessageForm
{
    [Display(Name = "المدرسة")]
    [Required(ErrorMessage = "المدرسة مطلوبة")]
    public int SchoolId { get; set; }

    [Display(Name = "نوع المستلم")]
    [Required(ErrorMessage = "نوع المستلم مطلوب")]
    public StudentMessageTargetType RecipientKind { get; set; } = StudentMessageTargetType.Student;

    [Display(Name = "الطالب")]
    public int? StudentId { get; set; }

    [Display(Name = "حساب الإدارة")]
    public string? ManagementUserId { get; set; }

    [Display(Name = "الموضوع")]
    [Required(ErrorMessage = "الموضوع مطلوب")]
    [MaxLength(250)]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "نص الرسالة")]
    [Required(ErrorMessage = "نص الرسالة مطلوب")]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    [Display(Name = "المرفقات")]
    public List<IFormFile>? Attachments { get; set; }
}

public class AdminReplyMessageForm
{
    public int ParentMessageId { get; set; }

    [Display(Name = "نص الرد")]
    [Required(ErrorMessage = "نص الرد مطلوب")]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    [Display(Name = "المرفقات")]
    public List<IFormFile>? Attachments { get; set; }
}
