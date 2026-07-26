using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentEducationalProfile : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? LearningDifficulties { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? BehaviourNotes { get; set; }
    public string? CounsellorRecommendations { get; set; }
    public string? SpecialEducationalNeeds { get; set; }
    public string? InterventionPlans { get; set; }
    public string? FollowUpRecords { get; set; }

    public Student? Student { get; set; }
}
