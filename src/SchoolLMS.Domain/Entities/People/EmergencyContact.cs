using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class EmergencyContact : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AlternativePhone { get; set; }
    public int PriorityOrder { get; set; } = 1;

    public Student? Student { get; set; }
}
