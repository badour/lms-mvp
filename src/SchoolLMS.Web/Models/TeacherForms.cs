using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Web.Models;

public class TeacherForm
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
    public string? FullNameEn { get; set; }

    [Required(ErrorMessage = "رقم الوثيقة مطلوب")]
    [Display(Name = "رقم الوثيقة / الهوية")]
    public string DocumentId { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الأب مطلوب")]
    [Display(Name = "اسم الأب")]
    public string ParentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الأم مطلوب")]
    [Display(Name = "اسم الأم")]
    public string MotherName { get; set; } = string.Empty;

    [Display(Name = "الجنس")]
    public Gender Gender { get; set; } = Gender.Male;

    [Display(Name = "الحالة الاجتماعية")]
    public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single;

    [Required(ErrorMessage = "الدور مطلوب")]
    [Display(Name = "الدور")]
    public string RoleName { get; set; } = "معلم";

    [Required(ErrorMessage = "الهاتف مطلوب")]
    [Display(Name = "الهاتف")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "البريد الإلكتروني")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "المدينة مطلوبة")]
    [Display(Name = "المدينة")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "العنوان مطلوب")]
    [Display(Name = "العنوان")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "المعلومات التعليمية")]
    public string? EducationalInfo { get; set; }

    [Display(Name = "التخصص")]
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

    [Display(Name = "الشعب المرتبطة")]
    public List<int> ClassSectionIds { get; set; } = [];

    [Display(Name = "الدروس الإلكترونية المرتبطة")]
    public List<int> OnlineLessonIds { get; set; } = [];

    [Display(Name = "مرفق")]
    public IFormFile? Attachment { get; set; }

    public bool RemoveAttachment { get; set; }
    public string? ExistingAttachmentName { get; set; }

    [Display(Name = "إنشاء حساب دخول")]
    public bool CreateLoginAccount { get; set; } = true;

    [Display(Name = "اسم المستخدم")]
    public string? UserName { get; set; }

    [Display(Name = "كلمة المرور")]
    public string? Password { get; set; }
}

public class AttendanceCreateForm
{
    [Required(ErrorMessage = "المعلم مطلوب")]
    [Display(Name = "المعلم")]
    public int TeacherId { get; set; }

    [Required(ErrorMessage = "المرحلة مطلوبة")]
    [Display(Name = "المرحلة")]
    public int AcademicStageId { get; set; }

    [Required(ErrorMessage = "الشعبة مطلوبة")]
    [Display(Name = "الشعبة")]
    public int ClassSectionId { get; set; }

    [Required(ErrorMessage = "التاريخ مطلوب")]
    [Display(Name = "التاريخ")]
    [DataType(DataType.Date)]
    public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public List<AttendanceMarkInput> Students { get; set; } = [];
}

public class AttendanceMarkInput
{
    public int StudentId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public bool IsPresent { get; set; } = true;
}

public class ExamCreateForm
{
    [Required(ErrorMessage = "المعلم مطلوب")]
    [Display(Name = "المعلم")]
    public int TeacherId { get; set; }

    [Required(ErrorMessage = "المرحلة مطلوبة")]
    [Display(Name = "المرحلة")]
    public int AcademicStageId { get; set; }

    [Required(ErrorMessage = "الشعبة مطلوبة")]
    [Display(Name = "الشعبة")]
    public int ClassSectionId { get; set; }

    [Required(ErrorMessage = "تاريخ ووقت الامتحان مطلوب")]
    [Display(Name = "تاريخ ووقت الامتحان")]
    [DataType(DataType.DateTime)]
    public DateTime ExamDateTime { get; set; } = DateTime.Now.AddDays(1);

    [Display(Name = "ملاحظات الامتحان")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "تعليمات الامتحان")]
    [MaxLength(4000)]
    public string? Instructions { get; set; }

    [Display(Name = "حالة الامتحان")]
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;
}
