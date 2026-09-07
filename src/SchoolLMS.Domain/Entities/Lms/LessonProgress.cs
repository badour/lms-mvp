using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Lms;

public class LessonProgress : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int StudentId { get; set; }
    public bool Opened { get; set; }
    public bool VideoStarted { get; set; }
    public decimal VideoCompletionPercent { get; set; }
    public bool FilesDownloaded { get; set; }
    public bool ActivityCompleted { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime? LastAccessedAt { get; set; }

    public Lesson? Lesson { get; set; }
}
