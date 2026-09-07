namespace SchoolLMS.Domain.Entities.Lms;

public class QuizQuestion
{
    public int QuizId { get; set; }
    public int QuestionId { get; set; }
    public decimal Marks { get; set; }
    public int SortOrder { get; set; }

    public Quiz? Quiz { get; set; }
    public Question? Question { get; set; }
}
