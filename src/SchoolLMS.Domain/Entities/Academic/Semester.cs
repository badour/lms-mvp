using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class Semester : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }

    public AcademicYear? AcademicYear { get; set; }
}
