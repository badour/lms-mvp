namespace SchoolLMS.Domain.Entities.Identity;

public class UserSchoolRole
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SchoolId { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
