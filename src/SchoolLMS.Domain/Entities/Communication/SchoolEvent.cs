using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class SchoolEvent : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public string? Location { get; set; }
    public string? Organizer { get; set; }
    public bool RequiresRegistration { get; set; }
    public int? Capacity { get; set; }
    public bool IsPublished { get; set; }
}
