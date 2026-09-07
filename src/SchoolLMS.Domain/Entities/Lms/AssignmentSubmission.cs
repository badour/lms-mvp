using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Lms;

public class AssignmentSubmission : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public string? TextResponse { get; set; }
    public string? LinkUrl { get; set; }
    public string? FilePath { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;
    public decimal? Score { get; set; }
    public string? TeacherFeedback { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Assignment? Assignment { get; set; }
}
