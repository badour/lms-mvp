using System.ComponentModel.DataAnnotations;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Web.Models;

public class CreateLessonForm
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

    [Display(Name = "فيديو الدرس")]
    public IFormFile? Video { get; set; }

    [Display(Name = "مواد الدرس")]
    public List<IFormFile>? Materials { get; set; }

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
}
