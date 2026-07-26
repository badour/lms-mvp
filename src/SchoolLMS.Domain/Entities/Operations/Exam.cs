using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Operations;

public class Exam : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int ExamPeriodId { get; set; }
    public int SubjectId { get; set; }
    public int? GradeLevelId { get; set; }
    public int? ClassSectionId { get; set; }
    public ExamType ExamType { get; set; }
    public DateOnly ExamDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? ClassroomId { get; set; }
    public decimal MaxScore { get; set; }
    public decimal PassScore { get; set; }
    public string? Topics { get; set; }
    public string? Instructions { get; set; }
    public DateTime? ResultPublishAt { get; set; }
    public bool IsPublished { get; set; }

    public ExamPeriod? ExamPeriod { get; set; }
}
