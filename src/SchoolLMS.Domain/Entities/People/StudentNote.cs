using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentNote : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string NoteText { get; set; } = string.Empty;
    public DateTime NoteDate { get; set; } = DateTime.UtcNow;
    public int SortOrder { get; set; }

    public Student? Student { get; set; }
}
