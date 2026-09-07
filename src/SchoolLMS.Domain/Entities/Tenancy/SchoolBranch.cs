using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Tenancy;

public class SchoolBranch : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    public School? School { get; set; }
}
