using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class AcademicStage : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? YearName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<GradeLevel> GradeLevels { get; set; } = new List<GradeLevel>();
}
