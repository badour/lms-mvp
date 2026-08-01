using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Operations;

public class Exam : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int ExamPeriodId { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? GradeLevelId { get; set; }
    public int? AcademicStageId { get; set; }
    public int? ClassSectionId { get; set; }
    public ExamType ExamType { get; set; } = ExamType.DailyQuiz;
    public DateOnly ExamDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? ClassroomId { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public decimal PassScore { get; set; } = 50;
    public string? Topics { get; set; }
    public string? Notes { get; set; }
    public string? Instructions { get; set; }
    public DateTime? ResultPublishAt { get; set; }
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;
    public bool IsPublished { get; set; }

    public ExamPeriod? ExamPeriod { get; set; }
}
