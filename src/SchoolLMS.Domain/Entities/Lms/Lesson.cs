using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Lms;

public class Lesson : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int CourseUnitId { get; set; }
    public int? TeacherId { get; set; }
    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string? Description { get; set; }
    public string? LearningObjectives { get; set; }
    public int SortOrder { get; set; }
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public string? FeaturedImagePath { get; set; }
    public string? VideoUrl { get; set; }
    public string? TeacherNotes { get; set; }
    public string? StudentInstructions { get; set; }

    public CourseUnit? CourseUnit { get; set; }
    public ICollection<LessonResource> Resources { get; set; } = new List<LessonResource>();
}
