using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Attendance;

namespace SchoolLMS.Application.Services.Attendance;

public interface IAttendanceAdminService
{
    Task<IReadOnlyList<AttendanceSessionListItemDto>> ListRecentAsync(int? schoolId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceStudentRowDto>> GetRosterAsync(AttendanceRosterRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> SaveAsync(SaveAttendanceRequest request, CancellationToken cancellationToken = default);
}
