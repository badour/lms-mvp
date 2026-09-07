using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Operations;

public class GradeItem : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int GradeCategoryId { get; set; }
    public int AcademicYearId { get; set; }
    public int SubjectId { get; set; }
    public int? ClassSectionId { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public DateOnly? DueDate { get; set; }

    public GradeCategory? GradeCategory { get; set; }
}
