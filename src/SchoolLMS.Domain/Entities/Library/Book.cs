using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Library;

public class Book : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Category { get; set; }
    public string? Isbn { get; set; }
    public bool IsActive { get; set; } = true;
}
