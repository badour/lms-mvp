using System.ComponentModel.DataAnnotations;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Exams;

public class ExamListItemDto
{
    public int Id { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string LessonNameAr { get; set; } = string.Empty;
    public string TeacherNameAr { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public string ClassNameAr { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public string? Notes { get; set; }
    public PublicationStatus Status { get; set; }
}

public class StudentExamItemDto
{
    public int Id { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string LessonNameAr { get; set; } = string.Empty;
    public string TeacherNameAr { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public string ClassNameAr { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public string? Notes { get; set; }
    public string? Instructions { get; set; }
}

public class CreateExamRequest
{
    [Required(ErrorMessage = "المدرسة مطلوبة")]
    [Display(Name = "اسم المدرسة")]
    public int SchoolId { get; set; }

    [Required(ErrorMessage = "الفترة الزمنية مطلوبة")]
    [Display(Name = "الفترة الزمنية")]
    public int TeachingPeriodId { get; set; }

    [Required(ErrorMessage = "اسم الدرس مطلوب")]
    [Display(Name = "اسم الدرس")]
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "المعلم مطلوب")]
    [Display(Name = "اسم المعلم")]
    public int TeacherId { get; set; }

    [Required(ErrorMessage = "المرحلة مطلوبة")]
    [Display(Name = "اسم المرحلة")]
    public int AcademicStageId { get; set; }

    [Required(ErrorMessage = "اسم الصف مطلوب")]
    [Display(Name = "اسم الصف")]
    public int ClassSectionId { get; set; }

    [Required(ErrorMessage = "تاريخ الامتحان مطلوب")]
    [Display(Name = "تاريخ الامتحان")]
    [DataType(DataType.Date)]
    public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

    [Display(Name = "ملاحظات")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "تعليمات الامتحان")]
    [MaxLength(4000)]
    public string? Instructions { get; set; }

    [Display(Name = "حالة الامتحان")]
    public PublicationStatus Status { get; set; } = PublicationStatus.Published;
}
