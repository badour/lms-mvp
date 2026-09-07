using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Entities.Tenancy;

namespace SchoolLMS.Domain.Entities.Identity;

public class UserSchoolAssignment : AuditableEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SchoolId { get; set; }
    public int? SchoolBranchId { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser? User { get; set; }
    public School? School { get; set; }
    public SchoolBranch? SchoolBranch { get; set; }
}
