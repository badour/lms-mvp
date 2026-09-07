using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class NotificationTemplate : AuditableEntity
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string BodyAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string? BodyEn { get; set; }
    public string Channel { get; set; } = "InApp";
}
