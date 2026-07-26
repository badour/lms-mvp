using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Operations;

public class AttendanceExcuse : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentAttendanceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
