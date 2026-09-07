using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Communication;

public class Badge : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ImagePath { get; set; }
    public string Category { get; set; } = "Academic";
    public int Points { get; set; }
    public bool IsActive { get; set; } = true;
}
