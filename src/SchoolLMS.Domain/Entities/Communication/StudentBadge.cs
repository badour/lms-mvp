using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class StudentBadge : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int BadgeId { get; set; }
    public int AcademicYearId { get; set; }
    public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
    public string AwardedByUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public bool VisibleToParent { get; set; } = true;
    public bool VisibleToStudent { get; set; } = true;

    public Badge? Badge { get; set; }
}
