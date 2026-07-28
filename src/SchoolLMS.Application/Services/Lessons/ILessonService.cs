using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Lessons;

namespace SchoolLMS.Application.Services.Lessons;

public interface ILessonService
{
    Task<PagedResult<LessonListItemDto>> SearchAsync(LessonSearchRequest request, CancellationToken cancellationToken = default);
    Task<LessonDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(UpdateLessonRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<LessonFileDto?> GetVideoFileAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<LessonFileDto?> GetMaterialFileAsync(int materialId, CancellationToken cancellationToken = default);
}
