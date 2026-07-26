using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Academic;

public class AcademicYear : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public AcademicYearStatus Status { get; set; } = AcademicYearStatus.Draft;
    public bool IsCurrent { get; set; }

    public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
}
