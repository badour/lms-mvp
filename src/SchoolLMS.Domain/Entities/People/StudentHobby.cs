using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentHobby : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Student? Student { get; set; }
}
