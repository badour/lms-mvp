using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class Classroom : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int? SchoolBranchId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Capacity { get; set; } = 30;
    public bool IsActive { get; set; } = true;
}
