using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Students;

namespace SchoolLMS.Application.Services.Students;

public interface IQimamCertificateService
{
    Task<PagedResult<QimamCertificateListItemDto>> SearchAsync(QimamCertificateSearchRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<QimamCertificateListItemDto>> GetForCurrentStudentAsync(CancellationToken cancellationToken = default);
    Task<QimamCertificateListItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateQimamCertificateRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<QimamCertificateFileDto?> GetFileAsync(int id, string kind, CancellationToken cancellationToken = default);
}
