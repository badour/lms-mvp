using System.ComponentModel.DataAnnotations;

namespace SchoolLMS.Application.DTOs.Routines;

public class RoutineLessonListItemDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string LessonName { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public int SessionsPerYear { get; set; }
}

public class CreateRoutineLessonRequest
{
    [Required(ErrorMessage = "المدرسة مطلوبة")]
    [Display(Name = "اسم المدرسة")]
    public int SchoolId { get; set; }

    [Required(ErrorMessage = "اسم الدرس مطلوب")]
    [Display(Name = "اسم الدرس")]
    [MaxLength(200)]
    public string LessonName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم المرحلة مطلوب")]
    [Display(Name = "اسم المرحلة")]
    public int GradeLevelId { get; set; }

    [Required(ErrorMessage = "عدد الحصص خلال السنة مطلوب")]
    [Display(Name = "عدد الحصص خلال السنة")]
    [Range(1, 500, ErrorMessage = "عدد الحصص يجب أن يكون بين 1 و 500")]
    public int SessionsPerYear { get; set; } = 1;
}
