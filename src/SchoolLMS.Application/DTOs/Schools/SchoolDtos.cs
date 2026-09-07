using System.ComponentModel.DataAnnotations;

namespace SchoolLMS.Application.DTOs.Schools;

public class SchoolListItemDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string SchoolType { get; set; } = string.Empty;
    public string GenderType { get; set; } = string.Empty;
    public string Currency { get; set; } = "IQD";
    public string? YearName { get; set; }
    public bool IsActive { get; set; }
    public int BranchCount { get; set; }
    public int StudentCount { get; set; }
    public int StageCount { get; set; }
    public int ActiveStageCount { get; set; }
}

public class SchoolStageItemDto
{
    public int? Id { get; set; }

    [Display(Name = "اسم المرحلة")]
    [Required(ErrorMessage = "اسم المرحلة مطلوب")]
    [MaxLength(200)]
    public string StageName { get; set; } = string.Empty;

    [Display(Name = "اسم الصف")]
    [Required(ErrorMessage = "اسم الصف مطلوب")]
    [MaxLength(200)]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "الشعبة")]
    [Required(ErrorMessage = "الشعبة مطلوبة")]
    [MaxLength(50)]
    public string SectionName { get; set; } = "أ";

    [Display(Name = "اسم المدرسة")]
    [Required(ErrorMessage = "اسم المدرسة مطلوب")]
    public int SchoolId { get; set; }

    [Display(Name = "اسم السنة")]
    [MaxLength(100)]
    public string? YearName { get; set; }

    [Display(Name = "حالة المرحلة")]
    public bool IsActive { get; set; } = true;

    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }
}

public class SchoolDetailsDto : SchoolListItemDto
{
    public string? LogoPath { get; set; }
    public string? SecondaryPhone { get; set; }
    public List<SchoolStageItemDto> Stages { get; set; } = [];
}

public class CreateSchoolRequest
{
    [Required(ErrorMessage = "اسم المدرسة بالعربية مطلوب")]
    [Display(Name = "اسم المدرسة (عربي)")]
    [MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم المدرسة بالإنجليزية مطلوب")]
    [Display(Name = "اسم المدرسة (إنجليزي)")]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Display(Name = "البريد الإلكتروني")]
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب للمراسلات")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "العنوان")]
    [Required(ErrorMessage = "العنوان مطلوب")]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "الهاتف")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Display(Name = "نوع المدرسة")]
    [Required(ErrorMessage = "نوع المدرسة مطلوب")]
    public string SchoolType { get; set; } = "Private";

    [Display(Name = "نوع الجنس")]
    [Required(ErrorMessage = "نوع الجنس مطلوب")]
    public string GenderType { get; set; } = "CoEducational";

    [Display(Name = "اسم السنة الدراسية")]
    [Required(ErrorMessage = "اسم السنة مطلوب")]
    [MaxLength(100)]
    public string YearName { get; set; } = "2025-2026";

    [Display(Name = "العملة")]
    [Required(ErrorMessage = "العملة مطلوبة")]
    [MaxLength(10)]
    public string Currency { get; set; } = "IQD";

    [Display(Name = "المراحل الدراسية")]
    public List<SchoolStageItemDto> Stages { get; set; } = [];
}

public class UpdateSchoolRequest : CreateSchoolRequest
{
    public int Id { get; set; }

    [Display(Name = "نشطة")]
    public bool IsActive { get; set; } = true;
}
