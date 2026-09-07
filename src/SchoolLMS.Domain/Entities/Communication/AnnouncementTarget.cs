namespace SchoolLMS.Domain.Entities.Communication;

public class AnnouncementTarget
{
    public int Id { get; set; }
    public int AnnouncementId { get; set; }
    public string TargetType { get; set; } = "Role";
    public string? TargetValue { get; set; }
    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }

    public Announcement? Announcement { get; set; }
}
