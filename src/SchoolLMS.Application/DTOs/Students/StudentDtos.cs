using System.ComponentModel.DataAnnotations;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Students;

public class StudentListItemDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public Gender Gender { get; set; }
    public StudentStatus Status { get; set; }
    public string? GradeNameAr { get; set; }
    public string? SectionNameAr { get; set; }
}

public class StudentDetailsDto : StudentListItemDto
{
    public string? ProfileImagePath { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? NationalId { get; set; }
    public DateOnly? RegistrationDate { get; set; }
    public string? PreviousSchool { get; set; }
    public string? Notes { get; set; }
    public int? SchoolBranchId { get; set; }
}

public class CreateStudentRequest
{
    [Required]
    public int SchoolId { get; set; }

    public int? SchoolBranchId { get; set; }

    [Required, MaxLength(50)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string FullNameAr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FullNameEn { get; set; }

    [Required]
    public Gender Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Nationality { get; set; } = "عراقي";
    public string? NationalId { get; set; }
    public string? PreviousSchool { get; set; }
    public string? Notes { get; set; }

    public int? AcademicYearId { get; set; }
    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }
}

public class UpdateStudentRequest : CreateStudentRequest
{
    public int Id { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
}

public class StudentSearchRequest
{
    public int? SchoolId { get; set; }
    public string? Search { get; set; }
    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }
    public StudentStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
