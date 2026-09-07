using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class Employee : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public bool IsActive { get; set; } = true;
}
