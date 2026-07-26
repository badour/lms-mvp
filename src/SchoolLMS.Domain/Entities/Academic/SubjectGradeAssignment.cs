using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class SubjectGradeAssignment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int GradeLevelId { get; set; }
    public int WeeklyPeriods { get; set; } = 1;

    public Subject? Subject { get; set; }
    public GradeLevel? GradeLevel { get; set; }
}
