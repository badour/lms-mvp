using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Entities.People;

namespace SchoolLMS.Domain.Entities.Academic;

public class RoutineLesson : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int AcademicStageId { get; set; }
    public int SessionsPerYear { get; set; }

    public Teacher? Teacher { get; set; }
    public AcademicStage? AcademicStage { get; set; }
}
