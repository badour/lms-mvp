using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Operations;

public class StudentAttendance : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int AttendanceSessionId { get; set; }
    public int StudentId { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.NotRecorded;
    public TimeOnly? LateTime { get; set; }
    public TimeOnly? LeftEarlyTime { get; set; }
    public string? Notes { get; set; }

    public AttendanceSession? AttendanceSession { get; set; }
}
