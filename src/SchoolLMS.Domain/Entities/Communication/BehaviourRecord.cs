using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class BehaviourRecord : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int BehaviourCategoryId { get; set; }
    public DateOnly RecordDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string ReportedByUserId { get; set; } = string.Empty;
    public string? ActionTaken { get; set; }
    public int Points { get; set; }
    public bool ParentNotified { get; set; }
    public bool IsConfidential { get; set; }
    public string Status { get; set; } = "Open";
    public DateOnly? FollowUpDate { get; set; }

    public BehaviourCategory? BehaviourCategory { get; set; }
}
