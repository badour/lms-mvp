using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Lessons;

namespace SchoolLMS.Application.Services.Lessons;

public interface ILessonService
{
    Task<PagedResult<LessonListItemDto>> SearchAsync(LessonSearchRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken = default);
}
