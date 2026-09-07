using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Routines;

namespace SchoolLMS.Application.Services.Routines;

public interface IRoutineLessonService
{
    Task<IReadOnlyList<RoutineLessonListItemDto>> ListAsync(int? schoolId, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateRoutineLessonRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
