using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class Guardian : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public string? NationalId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? AlternativePhone { get; set; }
    public string? Email { get; set; }
    public string? Occupation { get; set; }
    public string? Workplace { get; set; }
    public string? Address { get; set; }
    public string? EducationLevel { get; set; }

    public ICollection<StudentGuardian> Students { get; set; } = new List<StudentGuardian>();
}
