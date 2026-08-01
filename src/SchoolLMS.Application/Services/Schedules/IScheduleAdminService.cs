using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Schedules;

namespace SchoolLMS.Application.Services.Schedules;

public interface IScheduleAdminService
{
    Task<IReadOnlyList<ScheduleListItemDto>> ListAsync(int? schoolId, CancellationToken cancellationToken = default);
    Task<ScheduleDetailsDto?> GetBySectionAsync(int classSectionId, CancellationToken cancellationToken = default);
    Task<ServiceResult> SaveAsync(SaveScheduleRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteBySectionAsync(int classSectionId, CancellationToken cancellationToken = default);
}
