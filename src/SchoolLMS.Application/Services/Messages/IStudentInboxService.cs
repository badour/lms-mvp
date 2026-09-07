using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Messages;

namespace SchoolLMS.Application.Services.Messages;

public interface IStudentInboxService
{
    Task<IReadOnlyList<StudentInboxMessageDto>> GetMyMessagesAsync(CancellationToken cancellationToken = default);
    Task<StudentInboxMessageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InstructorOptionDto>> GetMyInstructorsAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> SendAsync(SendStudentInboxMessageRequest request, CancellationToken cancellationToken = default);
}
