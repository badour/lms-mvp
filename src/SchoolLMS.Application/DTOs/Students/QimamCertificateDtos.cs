using System.ComponentModel.DataAnnotations;
using SchoolLMS.Application.DTOs.Common;

namespace SchoolLMS.Application.DTOs.Students;

public class QimamCertificateListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SchoolId { get; set; }
    public string StudentNameAr { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string SchoolNameAr { get; set; } = string.Empty;
    public string CertificateName { get; set; } = string.Empty;
    public DateOnly CertificateDate { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? ImageOriginalName { get; set; }
    public string? DocumentPath { get; set; }
    public string? DocumentOriginalName { get; set; }
    public string? Notes { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateQimamCertificateRequest
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

    [Display(Name = "اسم الصف")]
    [MaxLength(150)]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "ملاحظات الشهادة")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Display(Name = "وصف الشهادة")]
    [MaxLength(4000)]
    public string? Description { get; set; }

    public FileUploadInput? Image { get; set; }
    public FileUploadInput? Document { get; set; }
}

public class QimamCertificateSearchRequest
{
    public int? SchoolId { get; set; }
    public int? StudentId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class QimamCertificateFileDto
{
    public int CertificateId { get; set; }
    public int StudentId { get; set; }
    public int SchoolId { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public bool IsImage { get; set; }
}
