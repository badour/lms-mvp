namespace SchoolLMS.Domain.Entities.Lms;

public class QuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string TextAr { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }

    public Question? Question { get; set; }
}
