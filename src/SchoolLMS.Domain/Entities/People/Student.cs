using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.People;

public class Student : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int? SchoolBranchId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? ProfileImagePath { get; set; }
    public Gender Gender { get; set; } = Gender.Male;
    public DateOnly? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Nationality { get; set; } = "عراقي";
    public string? NationalId { get; set; }
    public string? PassportOrCardId { get; set; }
    public DateOnly? RegistrationDate { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public string? ClassClassification { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Address { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public string? PreviousSchool { get; set; }
    public string? Notes { get; set; }

    public ICollection<StudentGuardian> Guardians { get; set; } = new List<StudentGuardian>();
    public StudentHealthProfile? HealthProfile { get; set; }
    public StudentEducationalProfile? EducationalProfile { get; set; }
    public ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
    public ICollection<StudentHobby> Hobbies { get; set; } = new List<StudentHobby>();
    public ICollection<StudentNote> StudentNotes { get; set; } = new List<StudentNote>();
    public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    public ICollection<StudentAddress> Addresses { get; set; } = new List<StudentAddress>();
}
