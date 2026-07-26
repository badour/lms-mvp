using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Lms;

public class QuizAttempt : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public int StudentId { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public int? DurationUsedSeconds { get; set; }
    public decimal? Score { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }

    public Quiz? Quiz { get; set; }
}
