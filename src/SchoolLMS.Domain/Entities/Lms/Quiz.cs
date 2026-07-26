using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Lms;

public class Quiz : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? ClassSectionId { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public int AttemptsAllowed { get; set; } = 1;
    public bool RandomizeQuestions { get; set; }
    public bool RandomizeAnswers { get; set; }
    public int? QuestionsPerAttempt { get; set; }
    public decimal PassingMark { get; set; }
    public bool ShowResultImmediately { get; set; } = true;
    public bool ShowCorrectAnswers { get; set; }
    public bool NegativeMarking { get; set; }
    public string? AccessPassword { get; set; }
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;

    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
