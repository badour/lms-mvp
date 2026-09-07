namespace SchoolLMS.Domain.Entities.Identity;

public class RolePermission
{
    public string RoleId { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
