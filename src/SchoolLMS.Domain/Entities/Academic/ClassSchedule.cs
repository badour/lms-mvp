using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class ClassSchedule : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int ClassSectionId { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? ClassroomId { get; set; }
    public int TeachingPeriodId { get; set; }
    public byte DayOfWeek { get; set; }
    public string? EntryText { get; set; }

    public ClassSection? ClassSection { get; set; }
    public Subject? Subject { get; set; }
    public TeachingPeriod? TeachingPeriod { get; set; }
    public Classroom? Classroom { get; set; }
}
