using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Academic;

public class GradeLevel : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AcademicStageId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public AcademicStage? AcademicStage { get; set; }
    public ICollection<ClassSection> Sections { get; set; } = new List<ClassSection>();
}
