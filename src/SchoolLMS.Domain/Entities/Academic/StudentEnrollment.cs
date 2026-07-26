using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Academic;

public class StudentEnrollment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int AcademicYearId { get; set; }
    public int GradeLevelId { get; set; }
    public int ClassSectionId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public int? SeatNumber { get; set; }
    public int? SequenceInClass { get; set; }
    public DateOnly EnrollmentDate { get; set; }

    public AcademicYear? AcademicYear { get; set; }
    public GradeLevel? GradeLevel { get; set; }
    public ClassSection? ClassSection { get; set; }
}
