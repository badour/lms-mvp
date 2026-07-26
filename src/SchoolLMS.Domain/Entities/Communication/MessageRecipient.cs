namespace SchoolLMS.Domain.Entities.Communication;

public class MessageRecipient
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string RecipientUserId { get; set; } = string.Empty;
    public DateTime? ReadAt { get; set; }

    public Message? Message { get; set; }
}
