using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Lms;

public class LessonResource : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string ResourceType { get; set; } = "File";
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? RelativePath { get; set; }
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSizeBytes { get; set; }
    public int SortOrder { get; set; }

    public Lesson? Lesson { get; set; }
}
