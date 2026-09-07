using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class TeachingPeriod : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SortOrder { get; set; }
    public bool IsBreak { get; set; }
}
