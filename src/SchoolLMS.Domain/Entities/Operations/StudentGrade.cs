using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Operations;

public class StudentGrade : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int GradeItemId { get; set; }
    public int StudentId { get; set; }
    public decimal? Score { get; set; }
    public GradeWorkflowStatus Status { get; set; } = GradeWorkflowStatus.Draft;
    public string? Notes { get; set; }

    public GradeItem? GradeItem { get; set; }
}
