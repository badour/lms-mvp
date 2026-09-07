namespace SchoolLMS.Domain.Entities.People;

public class StudentGuardian
{
    public int StudentId { get; set; }
    public int GuardianId { get; set; }
    public string Relationship { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsFinanciallyResponsible { get; set; }
    public bool CanReceiveNotifications { get; set; } = true;
    public bool CanCollectStudent { get; set; } = true;

    public Student? Student { get; set; }
    public Guardian? Guardian { get; set; }
}
