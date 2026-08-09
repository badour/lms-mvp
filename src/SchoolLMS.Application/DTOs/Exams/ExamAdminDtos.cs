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

    [Required(ErrorMessage = "اسم الدرس مطلوب")]
    [Display(Name = "اسم الدرس")]
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "المعلم مطلوب")]
    [Display(Name = "اسم المعلم")]
    public int TeacherId { get; set; }

    [Required(ErrorMessage = "الشعبة مطلوبة")]
    [Display(Name = "الشعبة")]
    public int ClassSectionId { get; set; }

    [Required(ErrorMessage = "تاريخ ووقت الامتحان مطلوب")]
    [Display(Name = "تاريخ ووقت الامتحان")]
    [DataType(DataType.DateTime)]
    public DateTime ExamDateTime { get; set; } = DateTime.Today.AddDays(1).AddHours(9);

    [Display(Name = "ملاحظات")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "تعليمات الامتحان")]
    [MaxLength(4000)]
    public string? Instructions { get; set; }

    [Display(Name = "حالة الامتحان")]
    public PublicationStatus Status { get; set; } = PublicationStatus.Published;
}
