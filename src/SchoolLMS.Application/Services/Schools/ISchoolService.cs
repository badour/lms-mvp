using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Schools;

namespace SchoolLMS.Application.Services.Schools;

public interface ISchoolService
{
    Task<IReadOnlyList<SchoolListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SchoolDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateSchoolRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(UpdateSchoolRequest request, CancellationToken cancellationToken = default);
}
