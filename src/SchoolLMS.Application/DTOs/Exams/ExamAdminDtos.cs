using System.ComponentModel.DataAnnotations;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Exams;

public class ExamListItemDto
{
    public int Id { get; set; }
    public string TeacherNameAr { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public string SectionNameAr { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public string? Notes { get; set; }
    public PublicationStatus Status { get; set; }
}

public class CreateExamRequest
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
