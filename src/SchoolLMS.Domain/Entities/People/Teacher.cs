using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.People;

public class Teacher : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int? EmployeeId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public string? DocumentId { get; set; }
    public string? ParentName { get; set; }
    public string? MotherName { get; set; }
    public Gender Gender { get; set; } = Gender.Male;
    public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single;
    public string RoleName { get; set; } = "معلم";
    public string? Specialization { get; set; }
    public string? EducationalInfo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? AttachmentPath { get; set; }
    public string? AttachmentOriginalName { get; set; }
    public string? AttachmentContentType { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee? Employee { get; set; }
}
