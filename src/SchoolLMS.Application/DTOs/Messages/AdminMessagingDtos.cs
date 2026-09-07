using System.ComponentModel.DataAnnotations;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Messages;

public class AdminMessageListItemDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyPreview { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SourceLabelAr { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
    public string TargetDisplayName { get; set; } = string.Empty;
    public StudentMessageTargetType TargetType { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsReadByCurrentUser { get; set; }
    public bool IsReadByRecipient { get; set; }
    public int AttachmentCount { get; set; }
    public int ReplyCount { get; set; }
    public bool HasLegacyReply { get; set; }
}

public class MessageAttachmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
}

public class AdminMessageDetailsDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SourceLabelAr { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
    public string SenderUserId { get; set; } = string.Empty;
    public string TargetDisplayName { get; set; } = string.Empty;
    public StudentMessageTargetType TargetType { get; set; }
    public int? StudentId { get; set; }
    public string? StudentNameAr { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsReadByRecipient { get; set; }
    public DateTime? RecipientReadAt { get; set; }
    public string? LegacyReplyBody { get; set; }
    public DateTime? LegacyRepliedAt { get; set; }
    public List<MessageAttachmentDto> Attachments { get; set; } = [];
    public List<AdminMessageThreadItemDto> Thread { get; set; } = [];
}

public class AdminMessageThreadItemDto
{
    public int Id { get; set; }
    public string SenderDisplayName { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsFromCurrentUser { get; set; }
    public List<MessageAttachmentDto> Attachments { get; set; } = [];
}

public class AdminMessageSearchRequest
{
    public int? SchoolId { get; set; }
    public string? Search { get; set; }
    public string? Source { get; set; } // student | instructor | management | admin | all
    public bool? UnreadOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AdminSendMessageRequest
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

    public List<FileUploadInput> Attachments { get; set; } = [];
}

public class AdminReplyMessageRequest
{
    public int ParentMessageId { get; set; }

    [Display(Name = "نص الرد")]
    [Required(ErrorMessage = "نص الرد مطلوب")]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    public List<FileUploadInput> Attachments { get; set; } = [];
}

public class MessagingLookupItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Extra { get; set; }
}

public class MessageFileDto
{
    public int AttachmentId { get; set; }
    public int MessageId { get; set; }
    public int SchoolId { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
}
