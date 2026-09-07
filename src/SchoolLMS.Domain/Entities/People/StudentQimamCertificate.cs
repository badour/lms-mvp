using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentQimamCertificate : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string CertificateName { get; set; } = string.Empty;
    public DateOnly CertificateDate { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? ImageOriginalName { get; set; }
    public string? DocumentPath { get; set; }
    public string? DocumentOriginalName { get; set; }
    public string? Notes { get; set; }
    public string? Description { get; set; }

    public Student? Student { get; set; }
}
