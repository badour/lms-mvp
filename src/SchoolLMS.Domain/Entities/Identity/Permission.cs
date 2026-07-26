using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Identity;

public class Permission : AuditableEntity
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
}
