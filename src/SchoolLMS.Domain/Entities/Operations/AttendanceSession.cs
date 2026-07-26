using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Operations;

public class AttendanceSession : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int ClassSectionId { get; set; }
    public int? TeachingPeriodId { get; set; }
    public int? TeacherId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public PublicationStatus Status { get; set; } = PublicationStatus.Draft;

    public ICollection<StudentAttendance> Records { get; set; } = new List<StudentAttendance>();
}
