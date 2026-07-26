using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class ClassSection : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int GradeLevelId { get; set; }
    public int? SchoolBranchId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Capacity { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public GradeLevel? GradeLevel { get; set; }
}
