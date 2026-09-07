namespace SchoolLMS.Domain.Entities.Identity;

public class UserPermission
{
    public string UserId { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;
    public Permission? Permission { get; set; }
    public ApplicationUser? User { get; set; }
}
