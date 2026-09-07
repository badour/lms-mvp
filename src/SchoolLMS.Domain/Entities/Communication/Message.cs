using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Entities.People;

namespace SchoolLMS.Domain.Entities.Communication;

public class Message : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public string? SenderDisplayName { get; set; }
    public int? StudentId { get; set; }
    public int? ParentMessageId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = "StudentInbox";
    public StudentMessageTargetType TargetType { get; set; } = StudentMessageTargetType.SchoolManagement;
    public int? TeacherId { get; set; }
    public string? TargetDisplayName { get; set; }
    public bool IsArchived { get; set; }
    public string? ReplyBody { get; set; }
    public DateTime? RepliedAt { get; set; }
    public string? RepliedByUserId { get; set; }

    public Student? Student { get; set; }
    public Teacher? Teacher { get; set; }
    public Message? ParentMessage { get; set; }
    public ICollection<Message> Replies { get; set; } = new List<Message>();
    public ICollection<MessageRecipient> Recipients { get; set; } = new List<MessageRecipient>();
    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}
