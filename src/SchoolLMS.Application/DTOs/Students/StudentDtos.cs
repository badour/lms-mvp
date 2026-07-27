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
    public string? FatherName { get; set; }
    public string? Phone1 { get; set; }
    public Gender Gender { get; set; }
    public StudentStatus Status { get; set; }
    public string? GradeNameAr { get; set; }
    public string? SectionNameAr { get; set; }
}

public class StudentDetailsDto : StudentListItemDto
{
    public string? ProfileImagePath { get; set; }
    public string? MotherName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? NationalId { get; set; }
    public string? PassportOrCardId { get; set; }
    public DateOnly? RegistrationDate { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public string? ClassClassification { get; set; }
    public string? Phone2 { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Address { get; set; }
    public string? PreviousSchool { get; set; }
    public string? Notes { get; set; }
    public int? SchoolBranchId { get; set; }
    public string? BloodType { get; set; }
    public string? DiseaseHistory { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public List<string> Hobbies { get; set; } = [];
    public List<string> NotesList { get; set; } = [];
}

public class CreateStudentRequest
{
    [Required(ErrorMessage = "المدرسة مطلوبة")]
    [Display(Name = "المدرسة")]
    public int SchoolId { get; set; }

    public int? SchoolBranchId { get; set; }

    [Display(Name = "رقم الطالب")]
    [MaxLength(50)]
    public string? StudentNumber { get; set; }

    [Required(ErrorMessage = "اسم الطالب مطلوب")]
    [Display(Name = "اسم الطالب")]
    [MaxLength(200)]
    public string FullNameAr { get; set; } = string.Empty;

    [Display(Name = "اسم الأب / ولي الأمر")]
    [Required(ErrorMessage = "اسم الأب مطلوب")]
    [MaxLength(200)]
    public string FatherName { get; set; } = string.Empty;

    [Display(Name = "اسم الأم")]
    [Required(ErrorMessage = "اسم الأم مطلوب")]
    [MaxLength(200)]
    public string MotherName { get; set; } = string.Empty;

    [Display(Name = "رقم البطاقة / جواز السفر")]
    [Required(ErrorMessage = "رقم البطاقة أو جواز السفر مطلوب")]
    [MaxLength(100)]
    public string PassportOrCardId { get; set; } = string.Empty;

    [Display(Name = "الجنس")]
    public Gender Gender { get; set; } = Gender.Male;

    [Display(Name = "تاريخ الميلاد")]
    [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "تاريخ القبول")]
    [Required(ErrorMessage = "تاريخ القبول مطلوب")]
    [DataType(DataType.Date)]
    public DateOnly? AdmissionDate { get; set; }

    [Display(Name = "السنة الدراسية")]
    public int? AcademicYearId { get; set; }

    [Display(Name = "الصف")]
    [Required(ErrorMessage = "الصف مطلوب")]
    public int? GradeLevelId { get; set; }

    [Display(Name = "الشعبة / اسم الصف")]
    [Required(ErrorMessage = "الشعبة مطلوبة")]
    public int? ClassSectionId { get; set; }

    [Display(Name = "تصنيف الصف")]
    [MaxLength(100)]
    public string? ClassClassification { get; set; }

    [Display(Name = "رقم الهاتف 1")]
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [MaxLength(30)]
    public string Phone1 { get; set; } = string.Empty;

    [Display(Name = "رقم الهاتف 2")]
    [MaxLength(30)]
    public string? Phone2 { get; set; }

    [Display(Name = "المدينة")]
    [Required(ErrorMessage = "المدينة مطلوبة")]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Display(Name = "المنطقة")]
    [Required(ErrorMessage = "المنطقة مطلوبة")]
    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;

    [Display(Name = "العنوان")]
    [Required(ErrorMessage = "العنوان مطلوب")]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "ملاحظات عامة")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "اسم جهة الطوارئ")]
    [MaxLength(200)]
    public string? EmergencyContactName { get; set; }

    [Display(Name = "هاتف جهة الطوارئ")]
    [MaxLength(30)]
    public string? EmergencyContactPhone { get; set; }

    [Display(Name = "فصيلة الدم")]
    [Required(ErrorMessage = "فصيلة الدم مطلوبة")]
    [MaxLength(10)]
    public string BloodType { get; set; } = string.Empty;

    [Display(Name = "التاريخ المرضي")]
    [MaxLength(2000)]
    public string? DiseaseHistory { get; set; }

    [Display(Name = "الهوايات")]
    public List<string> Hobbies { get; set; } = [];

    [Display(Name = "قائمة الملاحظات")]
    public List<string> NotesList { get; set; } = [];

    public string? FullNameEn { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Nationality { get; set; } = "عراقي";
    public string? NationalId { get; set; }
    public string? PreviousSchool { get; set; }
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
