namespace SchoolLMS.Domain.Entities.Identity;

public class AuditLog
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public int? SchoolId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? Browser { get; set; }
    public string? Device { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; } = true;
    public string? FailureReason { get; set; }
}
