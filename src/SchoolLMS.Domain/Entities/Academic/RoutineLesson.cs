using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class RoutineLesson : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string LessonName { get; set; } = string.Empty;
    public int GradeLevelId { get; set; }
    public int SessionsPerYear { get; set; }

    public GradeLevel? GradeLevel { get; set; }
}
