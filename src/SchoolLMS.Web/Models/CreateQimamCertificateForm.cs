using System.ComponentModel.DataAnnotations;

namespace SchoolLMS.Web.Models;

public class CreateQimamCertificateForm
{
    [Display(Name = "الطالب")]
    [Required(ErrorMessage = "الطالب مطلوب")]
    public int StudentId { get; set; }

    [Display(Name = "اسم الشهادة")]
    [Required(ErrorMessage = "اسم الشهادة مطلوب")]
    [MaxLength(250)]
    public string CertificateName { get; set; } = string.Empty;

    [Display(Name = "تاريخ الشهادة")]
    [Required(ErrorMessage = "تاريخ الشهادة مطلوب")]
    [DataType(DataType.Date)]
    public DateOnly? CertificateDate { get; set; }

    [Display(Name = "الصف / المرحلة")]
    [Required(ErrorMessage = "اختر الصف من قائمة المراحل")]
    public int GradeLevelId { get; set; }

    [Display(Name = "الشعبة")]
    [Required(ErrorMessage = "اختر الشعبة")]
    public int ClassSectionId { get; set; }

    /// <summary>Resolved display value saved to the certificate (grade / section).</summary>
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "صورة الشهادة")]
    public IFormFile? Image { get; set; }

    [Display(Name = "مستند الشهادة")]
    public IFormFile? Document { get; set; }

    [Display(Name = "ملاحظات الشهادة")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "وصف الشهادة")]
    [MaxLength(4000)]
    public string? Description { get; set; }
}
