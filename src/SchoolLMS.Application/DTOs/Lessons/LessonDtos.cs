using System.ComponentModel.DataAnnotations;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Lessons;

public class LessonListItemDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string SubjectNameAr { get; set; } = string.Empty;
    public string? TeacherNameAr { get; set; }
    public string IncludedClassesText { get; set; } = string.Empty;
    public DateTime? LessonDateTime { get; set; }
    public PublicationStatus Status { get; set; }
    public bool IsPosted { get; set; }
    public bool HasVideo { get; set; }
    public int MaterialCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLessonRequest
{
    [Display(Name = "المدرسة")]
    [Required(ErrorMessage = "المدرسة مطلوبة")]
    public int SchoolId { get; set; }

    [Display(Name = "اسم الدرس / المادة")]
    [Required(ErrorMessage = "اسم الدرس مطلوب")]
    public int SubjectId { get; set; }

    [Display(Name = "المعلم")]
    [Required(ErrorMessage = "المعلم مطلوب")]
    public int TeacherId { get; set; }

    [Display(Name = "الشعب المشمولة")]
    public List<int> ClassSectionIds { get; set; } = [];

    [Display(Name = "وصف الدرس")]
    [MaxLength(4000)]
    public string? Description { get; set; }

    [Display(Name = "ملاحظات الدرس")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ ووقت الدرس")]
    [Required(ErrorMessage = "تاريخ ووقت الدرس مطلوب")]
    public DateTime? LessonDateTime { get; set; }

    [Display(Name = "حالة الدرس")]
    [Required(ErrorMessage = "حالة الدرس مطلوبة")]
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;

    [Display(Name = "منشور")]
    public bool IsPosted { get; set; }

    public FileUploadInput? Video { get; set; }
    public List<FileUploadInput> Materials { get; set; } = [];
}

public class LessonSearchRequest
{
    public int? SchoolId { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public PublicationStatus? Status { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
