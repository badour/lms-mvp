using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Lms;

public class CourseUnit : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int? GradeLevelId { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
