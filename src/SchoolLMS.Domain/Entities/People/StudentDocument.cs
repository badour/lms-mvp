using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentDocument : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string RelativePath { get; set; } = string.Empty;

    public Student? Student { get; set; }
}
