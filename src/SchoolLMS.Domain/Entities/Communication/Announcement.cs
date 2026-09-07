using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Communication;

public class Announcement : AuditableEntity
{
    public int Id { get; set; }
    public int? SchoolId { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string ContentAr { get; set; } = string.Empty;
    public string? ContentEn { get; set; }
    public string? ImagePath { get; set; }
    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;
    public DateTime PublishAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireAt { get; set; }
    public string? AuthorUserId { get; set; }
    public bool RequiresAcknowledgement { get; set; }
    public bool IsPinned { get; set; }
    public bool IsPublished { get; set; }

    public ICollection<AnnouncementTarget> Targets { get; set; } = new List<AnnouncementTarget>();
}
