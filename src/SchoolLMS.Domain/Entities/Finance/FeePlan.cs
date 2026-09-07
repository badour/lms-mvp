using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Finance;

public class FeePlan : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int FeeTypeId { get; set; }
    public int AcademicYearId { get; set; }
    public int? GradeLevelId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsActive { get; set; } = true;

    public FeeType? FeeType { get; set; }
}
