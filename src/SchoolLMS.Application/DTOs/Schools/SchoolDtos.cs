using System.ComponentModel.DataAnnotations;

namespace SchoolLMS.Application.DTOs.Schools;

public class SchoolListItemDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string SchoolType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int BranchCount { get; set; }
    public int StudentCount { get; set; }
}

public class SchoolDetailsDto : SchoolListItemDto
{
    public string? LogoPath { get; set; }
    public string? Address { get; set; }
    public string? SecondaryPhone { get; set; }
    public string GenderType { get; set; } = string.Empty;
    public string Currency { get; set; } = "IQD";
}

public class CreateSchoolRequest
{
    [Required, MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    public string SchoolType { get; set; } = "Private";
    public string GenderType { get; set; } = "CoEducational";
    public string Currency { get; set; } = "IQD";
}

public class UpdateSchoolRequest : CreateSchoolRequest
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}
