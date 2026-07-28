using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Messages;

namespace SchoolLMS.Application.Services.Messages;

public interface IAdminMessagingService
{
    Task<PagedResult<AdminMessageListItemDto>> SearchAsync(AdminMessageSearchRequest request, CancellationToken cancellationToken = default);
    Task<AdminMessageDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> SendAsync(AdminSendMessageRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> ReplyAsync(AdminReplyMessageRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MessagingLookupItemDto>> SearchStudentsAsync(int schoolId, string? term, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MessagingLookupItemDto>> GetManagementAccountsAsync(int schoolId, CancellationToken cancellationToken = default);
    Task<MessageFileDto?> GetAttachmentAsync(int attachmentId, CancellationToken cancellationToken = default);
}
