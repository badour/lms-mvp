using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class Teacher : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int? EmployeeId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public string? Specialization { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee? Employee { get; set; }
}
