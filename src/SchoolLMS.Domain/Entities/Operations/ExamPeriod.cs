using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Operations;

public class ExamPeriod : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsPublished { get; set; }
}
