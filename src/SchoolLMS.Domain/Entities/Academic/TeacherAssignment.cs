using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class TeacherAssignment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public int AcademicYearId { get; set; }
    public int SubjectId { get; set; }
    public int ClassSectionId { get; set; }
    public bool IsActive { get; set; } = true;
}
