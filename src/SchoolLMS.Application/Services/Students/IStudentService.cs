using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Students;

namespace SchoolLMS.Application.Services.Students;

public interface IStudentService
{
    Task<PagedResult<StudentListItemDto>> SearchAsync(StudentSearchRequest request, CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateAsync(UpdateStudentRequest request, CancellationToken cancellationToken = default);
}
