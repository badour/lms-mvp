using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Messages;
using SchoolLMS.Domain.Entities.Communication;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Messages;

public class AdminMessagingService : IAdminMessagingService
{
    private static readonly HashSet<string> AttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg", ".zip", ".txt", ".xls", ".xlsx"
    };

    private const long MaxAttachmentBytes = 20 * 1024 * 1024;

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IUserDirectory _userDirectory;
    private readonly IFileStorage _fileStorage;
    private readonly IAuditService _audit;
    private readonly IValidator<AdminSendMessageRequest> _sendValidator;
    private readonly IValidator<AdminReplyMessageRequest> _replyValidator;

    public AdminMessagingService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IUserDirectory userDirectory,
        IFileStorage fileStorage,
        IAuditService audit,
        IValidator<AdminSendMessageRequest> sendValidator,
        IValidator<AdminReplyMessageRequest> replyValidator)
    {
        _db = db;
        _currentUser = currentUser;
        _userDirectory = userDirectory;
        _fileStorage = fileStorage;
        _audit = audit;
        _sendValidator = sendValidator;
        _replyValidator = replyValidator;
    }

    public async Task<PagedResult<AdminMessageListItemDto>> SearchAsync(AdminMessageSearchRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;
        var userId = _currentUser.UserId;

        var query = from m in _db.Messages.AsNoTracking()
                    where !m.IsDeleted && m.ParentMessageId == null
                    join school in _db.Schools.AsNoTracking() on m.SchoolId equals school.Id
                    select new { m, school };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.m.SchoolId));
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(x => x.m.SchoolId == request.SchoolId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.m.Subject.Contains(term) ||
                x.m.Body.Contains(term) ||
                (x.m.SenderDisplayName != null && x.m.SenderDisplayName.Contains(term)) ||
                (x.m.TargetDisplayName != null && x.m.TargetDisplayName.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.Source) && !string.Equals(request.Source, "all", StringComparison.OrdinalIgnoreCase))
        {
            query = request.Source.Trim().ToLowerInvariant() switch
            {
                "student" => query.Where(x => x.m.Category == "StudentInbox" || x.m.TargetType == StudentMessageTargetType.Student),
                "instructor" => query.Where(x => x.m.TargetType == StudentMessageTargetType.Instructor || x.m.TeacherId != null),
                "management" => query.Where(x => x.m.TargetType == StudentMessageTargetType.SchoolManagement),
                "admin" => query.Where(x => x.m.Category == "AdminContact" || x.m.TargetType == StudentMessageTargetType.SystemAdmin),
                _ => query
            };
        }

        if (request.UnreadOnly == true && !string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(x => _db.MessageRecipients.Any(r =>
                r.MessageId == x.m.Id &&
                r.RecipientUserId == userId &&
                r.ReadAt == null));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.m.Id,
                x.m.SchoolId,
                SchoolNameAr = x.school.NameAr,
                x.m.Subject,
                x.m.Body,
                x.m.Category,
                x.m.TargetType,
                x.m.SenderDisplayName,
                x.m.TargetDisplayName,
                x.m.SenderUserId,
                x.m.CreatedAt,
                x.m.ReplyBody,
                AttachmentCount = _db.MessageAttachments.Count(a => a.MessageId == x.m.Id && !a.IsDeleted),
                ReplyCount = _db.Messages.Count(r => r.ParentMessageId == x.m.Id && !r.IsDeleted),
                IsReadByCurrentUser = userId != null && _db.MessageRecipients.Any(r =>
                    r.MessageId == x.m.Id && r.RecipientUserId == userId && r.ReadAt != null),
                IsCurrentUserRecipient = userId != null && _db.MessageRecipients.Any(r =>
                    r.MessageId == x.m.Id && r.RecipientUserId == userId),
                IsReadByRecipient = _db.MessageRecipients.Any(r => r.MessageId == x.m.Id && r.ReadAt != null)
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(x => new AdminMessageListItemDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            SchoolNameAr = x.SchoolNameAr,
            Subject = x.Subject,
            BodyPreview = x.Body.Length <= 120 ? x.Body : x.Body[..120] + "…",
            Category = x.Category,
            SourceLabelAr = SourceLabel(x.Category, x.TargetType),
            SenderDisplayName = x.SenderDisplayName ?? "—",
            TargetDisplayName = x.TargetDisplayName ?? "—",
            TargetType = x.TargetType,
            CreatedAt = x.CreatedAt,
            IsReadByCurrentUser = !x.IsCurrentUserRecipient || x.IsReadByCurrentUser,
            IsReadByRecipient = x.IsReadByRecipient,
            AttachmentCount = x.AttachmentCount,
            ReplyCount = x.ReplyCount,
            HasLegacyReply = !string.IsNullOrWhiteSpace(x.ReplyBody)
        }).ToList();

        return new PagedResult<AdminMessageListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<AdminMessageDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await _db.Messages
            .Include(x => x.Recipients)
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (message is null)
        {
            return null;
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(message.SchoolId))
        {
            return null;
        }

        await MarkReadForCurrentUserAsync(message, cancellationToken);

        var schoolName = await _db.Schools.AsNoTracking()
            .Where(x => x.Id == message.SchoolId)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        string? studentName = null;
        if (message.StudentId.HasValue)
        {
            studentName = await _db.Students.AsNoTracking()
                .Where(x => x.Id == message.StudentId.Value)
                .Select(x => x.FullNameAr)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var replies = await _db.Messages
            .Include(x => x.Attachments)
            .Where(x => x.ParentMessageId == message.Id && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        foreach (var reply in replies)
        {
            await MarkReadForCurrentUserAsync(reply, cancellationToken);
        }

        var recipientRead = message.Recipients
            .Where(x => x.ReadAt != null)
            .OrderBy(x => x.ReadAt)
            .FirstOrDefault();

        return new AdminMessageDetailsDto
        {
            Id = message.Id,
            SchoolId = message.SchoolId,
            SchoolNameAr = schoolName,
            Subject = message.Subject,
            Body = message.Body,
            Category = message.Category,
            SourceLabelAr = SourceLabel(message.Category, message.TargetType),
            SenderDisplayName = message.SenderDisplayName ?? "—",
            SenderUserId = message.SenderUserId,
            TargetDisplayName = message.TargetDisplayName ?? "—",
            TargetType = message.TargetType,
            StudentId = message.StudentId,
            StudentNameAr = studentName,
            CreatedAt = message.CreatedAt,
            IsReadByRecipient = recipientRead != null,
            RecipientReadAt = recipientRead?.ReadAt,
            LegacyReplyBody = message.ReplyBody,
            LegacyRepliedAt = message.RepliedAt,
            Attachments = MapAttachments(message.Attachments),
            Thread = replies.Select(r => new AdminMessageThreadItemDto
            {
                Id = r.Id,
                SenderDisplayName = r.SenderDisplayName ?? "—",
                Body = r.Body,
                CreatedAt = r.CreatedAt,
                IsFromCurrentUser = r.SenderUserId == _currentUser.UserId,
                Attachments = MapAttachments(r.Attachments)
            }).ToList()
        };
    }

    public async Task<ServiceResult<int>> SendAsync(AdminSendMessageRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return ServiceResult<int>.Failure("يجب تسجيل الدخول.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(request.SchoolId))
        {
            return ServiceResult<int>.Failure("غير مصرح بالإرسال إلى هذه المدرسة.");
        }

        var validation = await _sendValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        foreach (var file in request.Attachments)
        {
            var err = ValidateAttachment(file);
            if (err is not null)
            {
                return ServiceResult<int>.Failure(err);
            }
        }

        string targetDisplay;
        var recipientIds = new List<string>();
        int? studentId = null;

        if (request.RecipientKind == StudentMessageTargetType.Student)
        {
            if (!request.StudentId.HasValue)
            {
                return ServiceResult<int>.Failure("اختر الطالب المستلم.");
            }

            // Prefer DB primary key; also accept الرقم (StudentNumber) from the Students grid first column.
            var student = await _db.Students.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.StudentId.Value && x.SchoolId == request.SchoolId && !x.IsDeleted, cancellationToken);
            if (student is null)
            {
                var studentNumber = request.StudentId.Value.ToString();
                student = await _db.Students.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.StudentNumber == studentNumber && x.SchoolId == request.SchoolId && !x.IsDeleted, cancellationToken);
            }

            if (student is null)
            {
                return ServiceResult<int>.Failure("الطالب غير موجود في هذه المدرسة.");
            }

            if (string.IsNullOrWhiteSpace(student.UserId))
            {
                return ServiceResult<int>.Failure("حساب الطالب غير مرتبط بمستخدم.");
            }

            studentId = student.Id;
            targetDisplay = student.FullNameAr;
            recipientIds.Add(student.UserId);
        }
        else if (request.RecipientKind == StudentMessageTargetType.SchoolManagement)
        {
            if (string.IsNullOrWhiteSpace(request.ManagementUserId))
            {
                return ServiceResult<int>.Failure("اختر حساب الإدارة المستلم.");
            }

            var managementAccounts = await _userDirectory.GetSchoolManagementAccountsAsync(request.SchoolId, cancellationToken);
            var account = managementAccounts.FirstOrDefault(x => x.UserId == request.ManagementUserId);
            if (account.UserId is null)
            {
                return ServiceResult<int>.Failure("حساب الإدارة غير صالح لهذه المدرسة.");
            }

            targetDisplay = account.DisplayName;
            recipientIds.Add(account.UserId);
        }
        else
        {
            return ServiceResult<int>.Failure("نوع المستلم غير مدعوم من لوحة الإدارة.");
        }

        var senderName = await _userDirectory.GetDisplayNameAsync(_currentUser.UserId, cancellationToken) ?? _currentUser.UserName ?? "الإدارة";
        var savedPaths = new List<string>();

        try
        {
            var message = new Message
            {
                SchoolId = request.SchoolId,
                SenderUserId = _currentUser.UserId,
                SenderDisplayName = senderName,
                StudentId = studentId,
                Subject = request.Subject.Trim(),
                Body = request.Body.Trim(),
                Category = "AdminContact",
                TargetType = request.RecipientKind,
                TargetDisplayName = targetDisplay,
                Recipients = recipientIds.Distinct().Select(id => new MessageRecipient { RecipientUserId = id }).ToList()
            };

            var sort = 1;
            foreach (var file in request.Attachments.Where(x => x.Length > 0))
            {
                var original = Path.GetFileName(file.FileName);
                var path = await _fileStorage.SaveAsync(file.Content, original, $"messages/{request.SchoolId}", cancellationToken);
                savedPaths.Add(path);
                message.Attachments.Add(new MessageAttachment
                {
                    SchoolId = request.SchoolId,
                    Title = Path.GetFileNameWithoutExtension(original),
                    OriginalFileName = original,
                    RelativePath = path,
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length
                });
                sort++;
            }

            _db.Messages.Add(message);
            await _db.SaveChangesAsync(cancellationToken);

            await _audit.LogAsync("Messaging.Send", nameof(Message), message.Id.ToString(),
                schoolId: message.SchoolId, cancellationToken: cancellationToken);

            return ServiceResult<int>.Success(message.Id);
        }
        catch
        {
            foreach (var path in savedPaths)
            {
                await _fileStorage.DeleteAsync(path, cancellationToken);
            }

            throw;
        }
    }

    public async Task<ServiceResult<int>> ReplyAsync(AdminReplyMessageRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return ServiceResult<int>.Failure("يجب تسجيل الدخول.");
        }

        var validation = await _replyValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var parent = await _db.Messages
            .FirstOrDefaultAsync(x => x.Id == request.ParentMessageId && !x.IsDeleted, cancellationToken);
        if (parent is null)
        {
            return ServiceResult<int>.Failure("الرسالة الأصلية غير موجودة.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(parent.SchoolId))
        {
            return ServiceResult<int>.Failure("غير مصرح بالرد على هذه الرسالة.");
        }

        foreach (var file in request.Attachments)
        {
            var err = ValidateAttachment(file);
            if (err is not null)
            {
                return ServiceResult<int>.Failure(err);
            }
        }

        var senderName = await _userDirectory.GetDisplayNameAsync(_currentUser.UserId, cancellationToken) ?? _currentUser.UserName ?? "الإدارة";
        var recipientIds = new List<string> { parent.SenderUserId };
        recipientIds.AddRange(await _db.MessageRecipients.AsNoTracking()
            .Where(x => x.MessageId == parent.Id)
            .Select(x => x.RecipientUserId)
            .ToListAsync(cancellationToken));
        recipientIds = recipientIds
            .Where(x => !string.IsNullOrWhiteSpace(x) && x != _currentUser.UserId)
            .Distinct()
            .ToList();

        if (recipientIds.Count == 0)
        {
            return ServiceResult<int>.Failure("لا يوجد مستلم للرد.");
        }

        var savedPaths = new List<string>();
        try
        {
            var reply = new Message
            {
                SchoolId = parent.SchoolId,
                SenderUserId = _currentUser.UserId,
                SenderDisplayName = senderName,
                StudentId = parent.StudentId,
                ParentMessageId = parent.Id,
                Subject = parent.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase) ? parent.Subject : $"Re: {parent.Subject}",
                Body = request.Body.Trim(),
                Category = "AdminContact",
                TargetType = parent.TargetType,
                TargetDisplayName = parent.SenderDisplayName ?? parent.TargetDisplayName,
                TeacherId = parent.TeacherId,
                Recipients = recipientIds.Select(id => new MessageRecipient { RecipientUserId = id }).ToList()
            };

            foreach (var file in request.Attachments.Where(x => x.Length > 0))
            {
                var original = Path.GetFileName(file.FileName);
                var path = await _fileStorage.SaveAsync(file.Content, original, $"messages/{parent.SchoolId}", cancellationToken);
                savedPaths.Add(path);
                reply.Attachments.Add(new MessageAttachment
                {
                    SchoolId = parent.SchoolId,
                    Title = Path.GetFileNameWithoutExtension(original),
                    OriginalFileName = original,
                    RelativePath = path,
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length
                });
            }

            parent.ReplyBody = request.Body.Trim();
            parent.RepliedAt = DateTime.UtcNow;
            parent.RepliedByUserId = _currentUser.UserId;

            _db.Messages.Add(reply);
            await _db.SaveChangesAsync(cancellationToken);

            await _audit.LogAsync("Messaging.Reply", nameof(Message), reply.Id.ToString(),
                schoolId: reply.SchoolId, cancellationToken: cancellationToken);

            return ServiceResult<int>.Success(reply.Id);
        }
        catch
        {
            foreach (var path in savedPaths)
            {
                await _fileStorage.DeleteAsync(path, cancellationToken);
            }

            throw;
        }
    }

    public async Task<IReadOnlyList<MessagingLookupItemDto>> SearchStudentsAsync(int schoolId, string? term, CancellationToken cancellationToken = default)
    {
        if ((!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(schoolId)) || schoolId <= 0)
        {
            return Array.Empty<MessagingLookupItemDto>();
        }

        var query = _db.Students.AsNoTracking()
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            var parsedId = int.TryParse(t, out var id) ? id : (int?)null;
            query = query.Where(x =>
                x.FullNameAr.Contains(t) ||
                x.StudentNumber.Contains(t) ||
                (parsedId.HasValue && x.Id == parsedId.Value) ||
                (x.FullNameEn != null && x.FullNameEn.Contains(t)));
        }

        return await query
            .OrderBy(x => x.StudentNumber)
            .ThenBy(x => x.FullNameAr)
            .Take(string.IsNullOrWhiteSpace(term) ? 500 : 30)
            .Select(x => new MessagingLookupItemDto
            {
                Id = x.Id.ToString(),
                Name = x.FullNameAr,
                Extra = x.StudentNumber
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MessagingLookupItemDto>> GetManagementAccountsAsync(int schoolId, CancellationToken cancellationToken = default)
    {
        if ((!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(schoolId)) || schoolId <= 0)
        {
            return Array.Empty<MessagingLookupItemDto>();
        }

        var accounts = await _userDirectory.GetSchoolManagementAccountsAsync(schoolId, cancellationToken);
        return accounts.Select(x => new MessagingLookupItemDto
        {
            Id = x.UserId,
            Name = x.DisplayName,
            Extra = x.UserName
        }).ToList();
    }

    public async Task<MessageFileDto?> GetAttachmentAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _db.MessageAttachments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == attachmentId && !x.IsDeleted, cancellationToken);
        if (attachment is null)
        {
            return null;
        }

        var message = await _db.Messages.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == attachment.MessageId && !x.IsDeleted, cancellationToken);
        if (message is null)
        {
            return null;
        }

        var isParticipant = !string.IsNullOrWhiteSpace(_currentUser.UserId) &&
            (message.SenderUserId == _currentUser.UserId ||
             await _db.MessageRecipients.AsNoTracking().AnyAsync(
                 r => r.MessageId == message.Id && r.RecipientUserId == _currentUser.UserId, cancellationToken));

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(message.SchoolId) && !isParticipant)
        {
            return null;
        }

        return new MessageFileDto
        {
            AttachmentId = attachment.Id,
            MessageId = message.Id,
            SchoolId = message.SchoolId,
            RelativePath = attachment.RelativePath,
            OriginalFileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType ?? "application/octet-stream"
        };
    }

    private async Task MarkReadForCurrentUserAsync(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return;
        }

        var recipient = message.Recipients.FirstOrDefault(x => x.RecipientUserId == _currentUser.UserId);
        if (recipient is null)
        {
            recipient = await _db.MessageRecipients
                .FirstOrDefaultAsync(x => x.MessageId == message.Id && x.RecipientUserId == _currentUser.UserId, cancellationToken);
        }

        if (recipient is not null && recipient.ReadAt is null)
        {
            recipient.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    private static List<MessageAttachmentDto> MapAttachments(IEnumerable<MessageAttachment> attachments) =>
        attachments.Where(x => !x.IsDeleted).Select(x => new MessageAttachmentDto
        {
            Id = x.Id,
            Title = x.Title,
            OriginalFileName = x.OriginalFileName,
            ContentType = x.ContentType,
            FileSizeBytes = x.FileSizeBytes
        }).ToList();

    private static string SourceLabel(string category, StudentMessageTargetType targetType)
    {
        if (string.Equals(category, "StudentInbox", StringComparison.OrdinalIgnoreCase))
        {
            return targetType switch
            {
                StudentMessageTargetType.Instructor => "من طالب → معلم",
                StudentMessageTargetType.SchoolManagement => "من طالب → إدارة المدرسة",
                StudentMessageTargetType.SystemAdmin => "من طالب → إدارة النظام",
                _ => "من طالب"
            };
        }

        return targetType switch
        {
            StudentMessageTargetType.Student => "من الإدارة → طالب",
            StudentMessageTargetType.SchoolManagement => "من الإدارة → إدارة المدرسة",
            StudentMessageTargetType.Instructor => "معلم",
            StudentMessageTargetType.SystemAdmin => "إدارة النظام",
            _ => "مراسلات"
        };
    }

    private static string? ValidateAttachment(FileUploadInput? file)
    {
        if (file is null || file.Length <= 0)
        {
            return null;
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AttachmentExtensions.Contains(extension))
        {
            return "مرفق غير مدعوم.";
        }

        if (file.Length > MaxAttachmentBytes)
        {
            return "حجم المرفق يتجاوز 20 ميجابايت.";
        }

        return null;
    }
}
