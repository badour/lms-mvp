using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Messages;
using SchoolLMS.Domain.Entities.Communication;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Messages;

public class StudentInboxService : IStudentInboxService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IUserDirectory _userDirectory;
    private readonly IAuditService _audit;
    private readonly IValidator<SendStudentInboxMessageRequest> _validator;

    public StudentInboxService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IUserDirectory userDirectory,
        IAuditService audit,
        IValidator<SendStudentInboxMessageRequest> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _userDirectory = userDirectory;
        _audit = audit;
        _validator = validator;
    }

    public async Task<IReadOnlyList<StudentInboxMessageDto>> GetMyMessagesAsync(CancellationToken cancellationToken = default)
    {
        var student = await GetCurrentStudentAsync(cancellationToken);
        if (student is null || string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return Array.Empty<StudentInboxMessageDto>();
        }

        var userId = _currentUser.UserId;

        var items = await _db.Messages.AsNoTracking()
            .Where(x => !x.IsDeleted
                        && x.ParentMessageId == null
                        && (
                            (x.Category == "StudentInbox" && (x.StudentId == student.Id || x.SenderUserId == userId))
                            || _db.MessageRecipients.Any(r => r.MessageId == x.Id && r.RecipientUserId == userId)
                        ))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StudentInboxMessageDto
            {
                Id = x.Id,
                Subject = x.Subject,
                Body = x.Body,
                TargetType = x.TargetType,
                TargetTypeNameAr = TargetTypeName(x.TargetType),
                TargetDisplayName = x.TargetDisplayName,
                SenderDisplayName = x.SenderDisplayName,
                TeacherNameAr = x.TeacherId == null
                    ? null
                    : _db.Teachers.Where(t => t.Id == x.TeacherId).Select(t => t.FullNameAr).FirstOrDefault(),
                CreatedAt = x.CreatedAt,
                HasReply = x.RepliedAt != null || _db.Messages.Any(r => r.ParentMessageId == x.Id && !r.IsDeleted),
                ReplyBody = x.ReplyBody,
                RepliedAt = x.RepliedAt,
                RecipientCount = _db.MessageRecipients.Count(r => r.MessageId == x.Id),
                IsIncoming = x.SenderUserId != userId,
                IsRead = !_db.MessageRecipients.Any(r =>
                    r.MessageId == x.Id && r.RecipientUserId == userId && r.ReadAt == null)
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<StudentInboxMessageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await GetCurrentStudentAsync(cancellationToken);
        if (student is null || string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return null;
        }

        var userId = _currentUser.UserId;

        var message = await _db.Messages
            .Include(x => x.Attachments)
            .Include(x => x.Recipients)
            .FirstOrDefaultAsync(x =>
                x.Id == id
                && !x.IsDeleted
                && (
                    (x.Category == "StudentInbox" && (x.StudentId == student.Id || x.SenderUserId == userId))
                    || x.Recipients.Any(r => r.RecipientUserId == userId)
                ), cancellationToken);

        if (message is null)
        {
            return null;
        }

        var recipient = message.Recipients.FirstOrDefault(r => r.RecipientUserId == userId);
        if (recipient is not null && recipient.ReadAt is null)
        {
            recipient.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }

        var reply = await _db.Messages.AsNoTracking()
            .Where(x => x.ParentMessageId == message.Id && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new { x.Body, x.CreatedAt })
            .FirstOrDefaultAsync(cancellationToken);

        return new StudentInboxMessageDto
        {
            Id = message.Id,
            Subject = message.Subject,
            Body = message.Body,
            TargetType = message.TargetType,
            TargetTypeNameAr = TargetTypeName(message.TargetType),
            TargetDisplayName = message.TargetDisplayName,
            SenderDisplayName = message.SenderDisplayName,
            TeacherNameAr = message.TeacherId == null
                ? null
                : await _db.Teachers.AsNoTracking()
                    .Where(t => t.Id == message.TeacherId)
                    .Select(t => t.FullNameAr)
                    .FirstOrDefaultAsync(cancellationToken),
            CreatedAt = message.CreatedAt,
            HasReply = message.RepliedAt != null || reply is not null,
            ReplyBody = reply?.Body ?? message.ReplyBody,
            RepliedAt = reply?.CreatedAt ?? message.RepliedAt,
            RecipientCount = message.Recipients.Count,
            IsIncoming = message.SenderUserId != userId,
            IsRead = true,
            Attachments = message.Attachments.Where(a => !a.IsDeleted).Select(a => new MessageAttachmentDto
            {
                Id = a.Id,
                Title = a.Title,
                OriginalFileName = a.OriginalFileName,
                ContentType = a.ContentType,
                FileSizeBytes = a.FileSizeBytes
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<InstructorOptionDto>> GetMyInstructorsAsync(CancellationToken cancellationToken = default)
    {
        var student = await GetCurrentStudentAsync(cancellationToken);
        if (student is null)
        {
            return Array.Empty<InstructorOptionDto>();
        }

        var sectionIds = await _db.StudentEnrollments.AsNoTracking()
            .Where(x => x.StudentId == student.Id && !x.IsDeleted && x.Status == EnrollmentStatus.Active)
            .Select(x => x.ClassSectionId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var query = _db.Teachers.AsNoTracking()
            .Where(t => t.SchoolId == student.SchoolId && t.IsActive && !t.IsDeleted);

        if (sectionIds.Count > 0)
        {
            var assignedTeacherIds = await _db.TeacherAssignments.AsNoTracking()
                .Where(a => a.SchoolId == student.SchoolId
                            && a.IsActive
                            && !a.IsDeleted
                            && sectionIds.Contains(a.ClassSectionId))
                .Select(a => a.TeacherId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (assignedTeacherIds.Count > 0)
            {
                query = query.Where(t => assignedTeacherIds.Contains(t.Id));
            }
        }

        return await query
            .OrderBy(t => t.FullNameAr)
            .Select(t => new InstructorOptionDto
            {
                TeacherId = t.Id,
                FullNameAr = t.FullNameAr,
                Specialization = t.Specialization
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult<int>> SendAsync(SendStudentInboxMessageRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return ServiceResult<int>.Failure("يجب تسجيل الدخول لإرسال الرسالة.");
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var student = await GetCurrentStudentProfileAsync(cancellationToken);
        if (student is null)
        {
            return ServiceResult<int>.Failure("حساب الطالب غير مرتبط بملف طالب.");
        }

        string targetDisplayName;
        var recipientUserIds = new List<string>();
        int? teacherId = null;

        switch (request.TargetType)
        {
            case StudentMessageTargetType.SystemAdmin:
                targetDisplayName = "إدارة النظام";
                recipientUserIds.AddRange(await _userDirectory.GetActiveUserIdsByRoleAsync(AppRoles.SuperAdministrator, cancellationToken));
                recipientUserIds.AddRange(await _userDirectory.GetActiveUserIdsByRoleAsync(AppRoles.CentralAdministrator, cancellationToken));
                break;

            case StudentMessageTargetType.SchoolManagement:
                targetDisplayName = "إدارة المدرسة";
                recipientUserIds.AddRange(await _userDirectory.GetActiveUserIdsByRoleForSchoolAsync(AppRoles.SchoolAdministrator, student.SchoolId, cancellationToken));
                if (recipientUserIds.Count == 0)
                {
                    recipientUserIds.AddRange(await _userDirectory.GetActiveUserIdsByRoleForSchoolAsync(AppRoles.AcademicSupervisor, student.SchoolId, cancellationToken));
                }

                if (recipientUserIds.Count == 0)
                {
                    // Fallback so student messages are never dropped in demo/dev.
                    recipientUserIds.AddRange(await _userDirectory.GetActiveUserIdsByRoleAsync(AppRoles.SuperAdministrator, cancellationToken));
                }
                break;

            case StudentMessageTargetType.Instructor:
                if (!request.TeacherId.HasValue || request.TeacherId.Value <= 0)
                {
                    return ServiceResult<int>.Failure("اختر المعلم المستلم.");
                }

                var teacher = await _db.Teachers.AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.Id == request.TeacherId.Value
                        && t.SchoolId == student.SchoolId
                        && t.IsActive
                        && !t.IsDeleted, cancellationToken);

                if (teacher is null)
                {
                    return ServiceResult<int>.Failure("المعلم المحدد غير متاح.");
                }

                if (string.IsNullOrWhiteSpace(teacher.UserId))
                {
                    return ServiceResult<int>.Failure("حساب المعلم غير مرتبط بمستخدم في النظام.");
                }

                teacherId = teacher.Id;
                targetDisplayName = teacher.FullNameAr;
                recipientUserIds.Add(teacher.UserId);
                break;

            default:
                return ServiceResult<int>.Failure("نوع المستلم غير صالح.");
        }

        recipientUserIds = recipientUserIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .Where(x => !string.Equals(x, _currentUser.UserId, StringComparison.Ordinal))
            .ToList();

        if (recipientUserIds.Count == 0)
        {
            return ServiceResult<int>.Failure("لا يوجد مستلم متاح لهذا النوع حالياً.");
        }

        var message = new Message
        {
            SchoolId = student.SchoolId,
            SenderUserId = _currentUser.UserId!,
            SenderDisplayName = student.FullNameAr,
            StudentId = student.Id,
            Subject = request.Subject.Trim(),
            Body = request.Body.Trim(),
            Category = "StudentInbox",
            TargetType = request.TargetType,
            TeacherId = teacherId,
            TargetDisplayName = targetDisplayName,
            Recipients = recipientUserIds.Select(userId => new MessageRecipient
            {
                RecipientUserId = userId
            }).ToList()
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync(
            "StudentInbox.Send",
            nameof(Message),
            message.Id.ToString(),
            newValues: new
            {
                message.TargetType,
                message.Subject,
                message.TeacherId,
                Recipients = recipientUserIds.Count
            },
            schoolId: message.SchoolId,
            cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(message.Id);
    }

    private async Task<StudentContext?> GetCurrentStudentAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return null;
        }

        return await _db.Students.AsNoTracking()
            .Where(x => x.UserId == _currentUser.UserId && !x.IsDeleted)
            .Select(x => new StudentContext(x.Id, x.SchoolId, x.FullNameAr))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Task<StudentContext?> GetCurrentStudentProfileAsync(CancellationToken cancellationToken) =>
        GetCurrentStudentAsync(cancellationToken);

    private static string TargetTypeName(StudentMessageTargetType type) => type switch
    {
        StudentMessageTargetType.SystemAdmin => "إدارة النظام",
        StudentMessageTargetType.SchoolManagement => "إدارة المدرسة",
        StudentMessageTargetType.Instructor => "المعلم",
        StudentMessageTargetType.Student => "الطالب",
        _ => type.ToString()
    };

    private sealed record StudentContext(int Id, int SchoolId, string FullNameAr);
}
