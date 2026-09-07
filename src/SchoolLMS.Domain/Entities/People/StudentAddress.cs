using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentAddress : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? Country { get; set; } = "العراق";
    public string? Governorate { get; set; }
    public string? District { get; set; }
    public string? Area { get; set; }
    public string? Street { get; set; }
    public string? NearestLandmark { get; set; }
    public string? DetailedAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public Student? Student { get; set; }
}
