using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Attendance;

namespace SchoolLMS.Application.Services.Attendance;

public interface IAttendanceAdminService
{
    Task<IReadOnlyList<AttendanceSessionListItemDto>> ListAsync(AttendanceSessionFilter filter, CancellationToken cancellationToken = default);
    Task<AttendanceSessionDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceStudentRowDto>> GetRosterAsync(AttendanceRosterRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> SaveAsync(SaveAttendanceRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
