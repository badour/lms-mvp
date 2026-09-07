using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities;

public class FileAttachment : AuditableEntity
{
    public long Id { get; set; }
    public int? SchoolId { get; set; }
    public string OwnerType { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string RelativePath { get; set; } = string.Empty;
}
