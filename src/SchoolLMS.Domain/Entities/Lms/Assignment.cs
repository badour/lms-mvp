using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Lms;

public class Assignment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int? SemesterId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public decimal TotalMarks { get; set; }
    public SubmissionType SubmissionType { get; set; } = SubmissionType.File;
    public bool AllowLateSubmission { get; set; }
    public decimal? LatePenaltyPercent { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public string? Instructions { get; set; }
    public string? AttachmentPath { get; set; }

    public ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
}
