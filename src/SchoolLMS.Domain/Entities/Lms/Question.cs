using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Lms;

public class Question : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int? CourseUnitId { get; set; }
    public string? Topic { get; set; }
    public string Difficulty { get; set; } = "Medium";
    public QuestionType QuestionType { get; set; }
    public string StemAr { get; set; } = string.Empty;
    public string? StemEn { get; set; }
    public decimal DefaultMarks { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
