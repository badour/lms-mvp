using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class MessageAttachment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }

    public Message? Message { get; set; }
}
