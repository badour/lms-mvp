using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Tenancy;

public class School : AuditableEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? Email { get; set; }
    public string SchoolType { get; set; } = "Private";
    public string GenderType { get; set; } = "CoEducational";
    public string Currency { get; set; } = "IQD";
    public bool IsActive { get; set; } = true;

    public SchoolSettings? Settings { get; set; }
    public ICollection<SchoolBranch> Branches { get; set; } = new List<SchoolBranch>();
}
