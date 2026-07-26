using SchoolLMS.Application.DTOs.Dashboards;

namespace SchoolLMS.Application.Services.Dashboards;

public interface IDashboardService
{
    Task<AdminDashboardDto> GetAdminDashboardAsync(int? schoolId = null, CancellationToken cancellationToken = default);
    Task<StudentDashboardDto?> GetStudentDashboardAsync(int studentId, CancellationToken cancellationToken = default);
    Task<TeacherDashboardDto?> GetTeacherDashboardAsync(int teacherId, CancellationToken cancellationToken = default);
    Task<ParentDashboardDto?> GetParentDashboardAsync(int guardianId, int? selectedStudentId = null, CancellationToken cancellationToken = default);
}
