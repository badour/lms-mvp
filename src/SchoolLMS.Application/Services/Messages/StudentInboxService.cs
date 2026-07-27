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

        var items = await _db.Messages.AsNoTracking()
            .Where(x => !x.IsDeleted
                        && x.Category == "StudentInbox"
                        && (x.StudentId == student.Id || x.SenderUserId == _currentUser.UserId))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StudentInboxMessageDto
            {
                Id = x.Id,
                Subject = x.Subject,
                Body = x.Body,
                TargetType = x.TargetType,
                TargetTypeNameAr = TargetTypeName(x.TargetType),
                TargetDisplayName = x.TargetDisplayName,
                TeacherNameAr = x.TeacherId == null
                    ? null
                    : _db.Teachers.Where(t => t.Id == x.TeacherId).Select(t => t.FullNameAr).FirstOrDefault(),
                CreatedAt = x.CreatedAt,
                HasReply = x.RepliedAt != null,
                ReplyBody = x.ReplyBody,
                RepliedAt = x.RepliedAt,
                RecipientCount = _db.MessageRecipients.Count(r => r.MessageId == x.Id)
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

        return await _db.Messages.AsNoTracking()
            .Where(x => x.Id == id
                        && !x.IsDeleted
                        && x.Category == "StudentInbox"
                        && (x.StudentId == student.Id || x.SenderUserId == _currentUser.UserId))
            .Select(x => new StudentInboxMessageDto
            {
                Id = x.Id,
                Subject = x.Subject,
                Body = x.Body,
                TargetType = x.TargetType,
                TargetTypeNameAr = TargetTypeName(x.TargetType),
                TargetDisplayName = x.TargetDisplayName,
                TeacherNameAr = x.TeacherId == null
                    ? null
                    : _db.Teachers.Where(t => t.Id == x.TeacherId).Select(t => t.FullNameAr).FirstOrDefault(),
                CreatedAt = x.CreatedAt,
                HasReply = x.RepliedAt != null,
                ReplyBody = x.ReplyBody,
                RepliedAt = x.RepliedAt,
                RecipientCount = _db.MessageRecipients.Count(r => r.MessageId == x.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);
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

        var student = await GetCurrentStudentAsync(cancellationToken);
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
            .Select(x => new StudentContext(x.Id, x.SchoolId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static string TargetTypeName(StudentMessageTargetType type) => type switch
    {
        StudentMessageTargetType.SystemAdmin => "إدارة النظام",
        StudentMessageTargetType.SchoolManagement => "إدارة المدرسة",
        StudentMessageTargetType.Instructor => "المعلم",
        _ => type.ToString()
    };

    private sealed record StudentContext(int Id, int SchoolId);
}
