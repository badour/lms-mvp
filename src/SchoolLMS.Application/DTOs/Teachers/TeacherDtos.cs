using System.ComponentModel.DataAnnotations;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Teachers;

public class TeacherListItemDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string? DocumentId { get; set; }
    public string? Phone { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public int ClassCount { get; set; }
    public int LessonCount { get; set; }
}

public class TeacherLessonLinkDto
{
    public int Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string SubjectNameAr { get; set; } = string.Empty;
    public bool HasVideo { get; set; }
    public int MaterialCount { get; set; }
}

public class TeacherClassLinkDto
{
    public int ClassSectionId { get; set; }
    public string StageNameAr { get; set; } = string.Empty;
    public string GradeNameAr { get; set; } = string.Empty;
    public string SectionNameAr { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectNameAr { get; set; } = string.Empty;
}

public class TeacherDetailsDto : TeacherListItemDto
{
    public string? FullNameEn { get; set; }
    public string? ParentName { get; set; }
    public string? MotherName { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
    public string? EducationalInfo { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? AttachmentOriginalName { get; set; }
    public bool HasAttachment { get; set; }
    public List<TeacherClassLinkDto> Classes { get; set; } = [];
    public List<TeacherLessonLinkDto> OnlineLessons { get; set; } = [];
}

public class TeacherSearchRequest
{
    public int? SchoolId { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TeacherUpsertRequest
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "المدرسة مطلوبة")]
    [Display(Name = "المدرسة")]
    public int SchoolId { get; set; }

    [Required(ErrorMessage = "اسم المعلم مطلوب")]
    [Display(Name = "اسم المعلم")]
    [MaxLength(200)]
    public string FullNameAr { get; set; } = string.Empty;

    [Display(Name = "الاسم بالإنجليزية")]
    [MaxLength(200)]
    public string? FullNameEn { get; set; }

    [Required(ErrorMessage = "رقم الوثيقة مطلوب")]
    [Display(Name = "رقم الوثيقة / الهوية")]
    [MaxLength(100)]
    public string DocumentId { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الأب مطلوب")]
    [Display(Name = "اسم الأب")]
    [MaxLength(200)]
    public string ParentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الأم مطلوب")]
    [Display(Name = "اسم الأم")]
    [MaxLength(200)]
    public string MotherName { get; set; } = string.Empty;

    [Display(Name = "الجنس")]
    public Gender Gender { get; set; } = Gender.Male;

    [Display(Name = "الحالة الاجتماعية")]
    public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single;

    [Display(Name = "الدور")]
    [Required(ErrorMessage = "الدور مطلوب")]
    [MaxLength(100)]
    public string RoleName { get; set; } = "معلم";

    [Display(Name = "الهاتف")]
    [Required(ErrorMessage = "الهاتف مطلوب")]
    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "البريد الإلكتروني")]
    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "البريد غير صالح")]
    public string? Email { get; set; }

    [Display(Name = "المدينة")]
    [Required(ErrorMessage = "المدينة مطلوبة")]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Display(Name = "العنوان")]
    [Required(ErrorMessage = "العنوان مطلوب")]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "المعلومات التعليمية")]
    [MaxLength(4000)]
    public string? EducationalInfo { get; set; }

    [Display(Name = "التخصص")]
    [MaxLength(200)]
    public string? Specialization { get; set; }

    [Display(Name = "تاريخ المباشرة")]
    [DataType(DataType.Date)]
    public DateOnly? StartDate { get; set; }

    [Display(Name = "تاريخ الانتهاء")]
    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "المادة للتخصيص")]
    public int? SubjectId { get; set; }

    [Display(Name = "السنة الدراسية")]
    public int? AcademicYearId { get; set; }

    [Display(Name = "المراحل والشعب المرتبطة")]
    public List<int> ClassSectionIds { get; set; } = [];

    [Display(Name = "الدروس الإلكترونية المرتبطة")]
    public List<int> OnlineLessonIds { get; set; } = [];

    public FileUploadInput? Attachment { get; set; }
    public bool RemoveAttachment { get; set; }

    [Display(Name = "إنشاء حساب دخول")]
    public bool CreateLoginAccount { get; set; } = true;

    [Display(Name = "اسم المستخدم")]
    [MaxLength(100)]
    public string? UserName { get; set; }

    [Display(Name = "كلمة المرور")]
    [MaxLength(100)]
    public string? Password { get; set; }
}
