using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Teachers;

namespace SchoolLMS.Application.Services.Teachers;

public interface ITeacherService
{
    Task<PagedResult<TeacherListItemDto>> SearchAsync(TeacherSearchRequest request, CancellationToken cancellationToken = default);
    Task<TeacherDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(TeacherUpsertRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(TeacherUpsertRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<(string RelativePath, string OriginalFileName, string ContentType)?> GetAttachmentAsync(int id, CancellationToken cancellationToken = default);
}
