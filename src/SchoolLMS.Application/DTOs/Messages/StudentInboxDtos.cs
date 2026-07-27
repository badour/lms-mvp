using System.ComponentModel.DataAnnotations;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Messages;

public class StudentInboxMessageDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public StudentMessageTargetType TargetType { get; set; }
    public string TargetTypeNameAr { get; set; } = string.Empty;
    public string? TargetDisplayName { get; set; }
    public string? TeacherNameAr { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasReply { get; set; }
    public string? ReplyBody { get; set; }
    public DateTime? RepliedAt { get; set; }
    public int RecipientCount { get; set; }
}

public class InstructorOptionDto
{
    public int TeacherId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string? Specialization { get; set; }
}

public class SendStudentInboxMessageRequest
{
    [Display(Name = "نوع المستلم")]
    [Required(ErrorMessage = "نوع المستلم مطلوب")]
    public StudentMessageTargetType TargetType { get; set; } = StudentMessageTargetType.SchoolManagement;

    [Display(Name = "المعلم")]
    public int? TeacherId { get; set; }

    [Display(Name = "الموضوع")]
    [Required(ErrorMessage = "الموضوع مطلوب")]
    [MaxLength(250)]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "نص الرسالة")]
    [Required(ErrorMessage = "نص الرسالة مطلوب")]
    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;
}

public class StudentInboxComposeModel
{
    public SendStudentInboxMessageRequest Request { get; set; } = new();
    public IReadOnlyList<InstructorOptionDto> Instructors { get; set; } = Array.Empty<InstructorOptionDto>();
}
