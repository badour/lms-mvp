using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Operations;

public class GradeCategory : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal WeightPercent { get; set; }
    public bool IsActive { get; set; } = true;
}
