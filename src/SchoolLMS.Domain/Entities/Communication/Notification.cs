using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Communication;

public class Notification : AuditableEntity
{
    public long Id { get; set; }
    public int? SchoolId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string BodyAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string? BodyEn { get; set; }
    public string Category { get; set; } = "General";
    public NotificationDeliveryStatus Status { get; set; } = NotificationDeliveryStatus.Pending;
    public DateTime? ReadAt { get; set; }
    public string? LinkUrl { get; set; }
}
