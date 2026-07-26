using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class Message : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public bool IsArchived { get; set; }

    public ICollection<MessageRecipient> Recipients { get; set; } = new List<MessageRecipient>();
}
