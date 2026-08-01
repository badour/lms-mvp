using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Teachers;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Entities.People;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Teachers;

public class TeacherService : ITeacherService
{
    private static readonly HashSet<string> AttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg", ".zip"
    };

    private const long MaxAttachmentBytes = 20 * 1024 * 1024;

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IFileStorage _fileStorage;
    private readonly IUserDirectory _userDirectory;
    private readonly IAuditService _audit;
    private readonly IValidator<TeacherUpsertRequest> _validator;

    public TeacherService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IFileStorage fileStorage,
        IUserDirectory userDirectory,
        IAuditService audit,
        IValidator<TeacherUpsertRequest> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _userDirectory = userDirectory;
        _audit = audit;
        _validator = validator;
    }

    public async Task<PagedResult<TeacherListItemDto>> SearchAsync(TeacherSearchRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query =
            from teacher in _db.Teachers.AsNoTracking()
            where !teacher.IsDeleted
            join school in _db.Schools.AsNoTracking() on teacher.SchoolId equals school.Id
            select new { teacher, school };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.teacher.SchoolId));
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(x => x.teacher.SchoolId == request.SchoolId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.teacher.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.teacher.FullNameAr.Contains(term) ||
                (x.teacher.DocumentId != null && x.teacher.DocumentId.Contains(term)) ||
                (x.teacher.Phone != null && x.teacher.Phone.Contains(term)) ||
                (x.teacher.Specialization != null && x.teacher.Specialization.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderBy(x => x.teacher.FullNameAr)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TeacherListItemDto
            {
                Id = x.teacher.Id,
                SchoolId = x.teacher.SchoolId,
                SchoolNameAr = x.school.NameAr,
                FullNameAr = x.teacher.FullNameAr,
                DocumentId = x.teacher.DocumentId,
                Phone = x.teacher.Phone,
                RoleName = x.teacher.RoleName,
                Specialization = x.teacher.Specialization,
                City = x.teacher.City,
                IsActive = x.teacher.IsActive,
                ClassCount = _db.TeacherAssignments.Count(a => a.TeacherId == x.teacher.Id && a.IsActive && !a.IsDeleted),
                LessonCount = _db.Lessons.Count(l => l.TeacherId == x.teacher.Id && !l.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TeacherListItemDto>
        {
            Items = rows,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<TeacherDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (teacher is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(teacher.SchoolId)))
        {
            return null;
        }

        var schoolName = await _db.Schools.AsNoTracking()
            .Where(x => x.Id == teacher.SchoolId)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var classes = await (
            from a in _db.TeacherAssignments.AsNoTracking()
            join section in _db.ClassSections.AsNoTracking() on a.ClassSectionId equals section.Id
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            join subject in _db.Subjects.AsNoTracking() on a.SubjectId equals subject.Id
            where a.TeacherId == teacher.Id && a.IsActive && !a.IsDeleted
            orderby stage.SortOrder, grade.SortOrder, section.NameAr
            select new TeacherClassLinkDto
            {
                ClassSectionId = section.Id,
                StageNameAr = stage.NameAr,
                GradeNameAr = grade.NameAr,
                SectionNameAr = section.NameAr,
                SubjectId = subject.Id,
                SubjectNameAr = subject.NameAr
            }
        ).ToListAsync(cancellationToken);

        var lessons = await (
            from lesson in _db.Lessons.AsNoTracking()
            join subject in _db.Subjects.AsNoTracking() on lesson.SubjectId equals subject.Id
            where lesson.TeacherId == teacher.Id && !lesson.IsDeleted
            orderby lesson.LessonDateTime descending, lesson.TitleAr
            select new TeacherLessonLinkDto
            {
                Id = lesson.Id,
                TitleAr = lesson.TitleAr,
                SubjectNameAr = subject.NameAr,
                HasVideo = lesson.VideoPath != null || lesson.VideoUrl != null,
                MaterialCount = _db.LessonResources.Count(r => r.LessonId == lesson.Id && !r.IsDeleted)
            }
        ).Take(50).ToListAsync(cancellationToken);

        return new TeacherDetailsDto
        {
            Id = teacher.Id,
            SchoolId = teacher.SchoolId,
            SchoolNameAr = schoolName,
            FullNameAr = teacher.FullNameAr,
            FullNameEn = teacher.FullNameEn,
            DocumentId = teacher.DocumentId,
            ParentName = teacher.ParentName,
            MotherName = teacher.MotherName,
            Gender = teacher.Gender,
            MaritalStatus = teacher.MaritalStatus,
            RoleName = teacher.RoleName,
            Specialization = teacher.Specialization,
            EducationalInfo = teacher.EducationalInfo,
            Phone = teacher.Phone,
            Email = teacher.Email,
            City = teacher.City,
            Address = teacher.Address,
            StartDate = teacher.StartDate,
            EndDate = teacher.EndDate,
            IsActive = teacher.IsActive,
            HasAttachment = !string.IsNullOrWhiteSpace(teacher.AttachmentPath),
            AttachmentOriginalName = teacher.AttachmentOriginalName,
            ClassCount = classes.Count,
            LessonCount = lessons.Count,
            Classes = classes,
            OnlineLessons = lessons
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(TeacherUpsertRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(request.SchoolId))
        {
            return ServiceResult<int>.Failure("غير مصرح بالإضافة إلى هذه المدرسة.");
        }

        if (!_currentUser.HasPermission(PermissionNames.TeachersCreate) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية إنشاء معلم.");
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var attachError = ValidateAttachment(request.Attachment);
        if (attachError is not null)
        {
            return ServiceResult<int>.Failure(attachError);
        }

        string? userId = null;
        if (request.CreateLoginAccount)
        {
            var userName = string.IsNullOrWhiteSpace(request.UserName)
                ? $"teacher_{request.SchoolId}_{Guid.NewGuid():N}"[..20]
                : request.UserName.Trim();
            var email = string.IsNullOrWhiteSpace(request.Email)
                ? $"{userName}@schoollms.local"
                : request.Email.Trim();
            var password = string.IsNullOrWhiteSpace(request.Password) ? "Teacher@12345" : request.Password;
            var created = await _userDirectory.CreateSchoolUserAsync(
                userName, email, request.FullNameAr.Trim(), password, AppRoles.Teacher, request.SchoolId, cancellationToken);
            if (!created.Succeeded)
            {
                return ServiceResult<int>.Failure(created.Errors);
            }

            userId = created.UserId;
        }

        string? attachmentPath = null;
        string? attachmentName = null;
        string? attachmentType = null;
        try
        {
            if (request.Attachment is { Length: > 0 })
            {
                attachmentPath = await _fileStorage.SaveAsync(
                    request.Attachment.Content,
                    Path.GetFileName(request.Attachment.FileName),
                    $"teachers/{request.SchoolId}",
                    cancellationToken);
                attachmentName = Path.GetFileName(request.Attachment.FileName);
                attachmentType = request.Attachment.ContentType;
            }

            var teacher = new Teacher
            {
                SchoolId = request.SchoolId,
                UserId = userId,
                FullNameAr = request.FullNameAr.Trim(),
                FullNameEn = request.FullNameEn?.Trim(),
                DocumentId = request.DocumentId.Trim(),
                ParentName = request.ParentName.Trim(),
                MotherName = request.MotherName.Trim(),
                Gender = request.Gender,
                MaritalStatus = request.MaritalStatus,
                RoleName = request.RoleName.Trim(),
                Specialization = request.Specialization?.Trim(),
                EducationalInfo = request.EducationalInfo?.Trim(),
                Phone = request.Phone.Trim(),
                Email = request.Email?.Trim(),
                City = request.City.Trim(),
                Address = request.Address.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                AttachmentPath = attachmentPath,
                AttachmentOriginalName = attachmentName,
                AttachmentContentType = attachmentType,
                IsActive = request.IsActive
            };

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync(cancellationToken);

            await SyncAssignmentsAsync(teacher, request, cancellationToken);
            await SyncLessonsAsync(teacher.Id, teacher.SchoolId, request.OnlineLessonIds, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _audit.LogAsync("Teachers.Create", nameof(Teacher), teacher.Id.ToString(),
                schoolId: teacher.SchoolId, cancellationToken: cancellationToken);

            return ServiceResult<int>.Success(teacher.Id);
        }
        catch
        {
            if (attachmentPath is not null)
            {
                await _fileStorage.DeleteAsync(attachmentPath, cancellationToken);
            }

            throw;
        }
    }

    public async Task<ServiceResult> UpdateAsync(TeacherUpsertRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.Id.HasValue)
        {
            return ServiceResult.Failure("معرّف المعلم مطلوب.");
        }

        var teacher = await _db.Teachers.FirstOrDefaultAsync(x => x.Id == request.Id.Value && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return ServiceResult.Failure("المعلم غير موجود.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(teacher.SchoolId))
        {
            return ServiceResult.Failure("غير مصرح بتعديل هذا المعلم.");
        }

        if (!_currentUser.HasPermission(PermissionNames.TeachersEdit) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية تعديل معلم.");
        }

        request.SchoolId = teacher.SchoolId;
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var attachError = ValidateAttachment(request.Attachment);
        if (attachError is not null)
        {
            return ServiceResult.Failure(attachError);
        }

        string? oldAttachment = null;
        if (request.RemoveAttachment && !string.IsNullOrWhiteSpace(teacher.AttachmentPath))
        {
            oldAttachment = teacher.AttachmentPath;
            teacher.AttachmentPath = null;
            teacher.AttachmentOriginalName = null;
            teacher.AttachmentContentType = null;
        }

        if (request.Attachment is { Length: > 0 })
        {
            oldAttachment ??= teacher.AttachmentPath;
            teacher.AttachmentPath = await _fileStorage.SaveAsync(
                request.Attachment.Content,
                Path.GetFileName(request.Attachment.FileName),
                $"teachers/{teacher.SchoolId}",
                cancellationToken);
            teacher.AttachmentOriginalName = Path.GetFileName(request.Attachment.FileName);
            teacher.AttachmentContentType = request.Attachment.ContentType;
        }

        teacher.FullNameAr = request.FullNameAr.Trim();
        teacher.FullNameEn = request.FullNameEn?.Trim();
        teacher.DocumentId = request.DocumentId.Trim();
        teacher.ParentName = request.ParentName.Trim();
        teacher.MotherName = request.MotherName.Trim();
        teacher.Gender = request.Gender;
        teacher.MaritalStatus = request.MaritalStatus;
        teacher.RoleName = request.RoleName.Trim();
        teacher.Specialization = request.Specialization?.Trim();
        teacher.EducationalInfo = request.EducationalInfo?.Trim();
        teacher.Phone = request.Phone.Trim();
        teacher.Email = request.Email?.Trim();
        teacher.City = request.City.Trim();
        teacher.Address = request.Address.Trim();
        teacher.StartDate = request.StartDate;
        teacher.EndDate = request.EndDate;
        teacher.IsActive = request.IsActive;

        await SyncAssignmentsAsync(teacher, request, cancellationToken);
        await SyncLessonsAsync(teacher.Id, teacher.SchoolId, request.OnlineLessonIds, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldAttachment) &&
            !string.Equals(oldAttachment, teacher.AttachmentPath, StringComparison.Ordinal))
        {
            await _fileStorage.DeleteAsync(oldAttachment, cancellationToken);
        }

        await _audit.LogAsync("Teachers.Update", nameof(Teacher), teacher.Id.ToString(),
            schoolId: teacher.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var teacher = await _db.Teachers.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return ServiceResult.Failure("المعلم غير موجود.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(teacher.SchoolId))
        {
            return ServiceResult.Failure("غير مصرح بحذف هذا المعلم.");
        }

        if (!_currentUser.HasPermission(PermissionNames.TeachersDelete) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية حذف معلم.");
        }

        teacher.IsDeleted = true;
        teacher.DeletedAt = DateTime.UtcNow;
        teacher.IsActive = false;

        var assignments = await _db.TeacherAssignments
            .Where(x => x.TeacherId == teacher.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var assignment in assignments)
        {
            assignment.IsActive = false;
            assignment.IsDeleted = true;
            assignment.DeletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(teacher.AttachmentPath))
        {
            await _fileStorage.DeleteAsync(teacher.AttachmentPath, cancellationToken);
            teacher.AttachmentPath = null;
            await _db.SaveChangesAsync(cancellationToken);
        }

        await _audit.LogAsync("Teachers.Delete", nameof(Teacher), teacher.Id.ToString(),
            schoolId: teacher.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<(string RelativePath, string OriginalFileName, string ContentType)?> GetAttachmentAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (teacher is null || string.IsNullOrWhiteSpace(teacher.AttachmentPath))
        {
            return null;
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(teacher.SchoolId))
        {
            return null;
        }

        return (teacher.AttachmentPath,
            teacher.AttachmentOriginalName ?? Path.GetFileName(teacher.AttachmentPath),
            teacher.AttachmentContentType ?? "application/octet-stream");
    }

    private async Task SyncAssignmentsAsync(Teacher teacher, TeacherUpsertRequest request, CancellationToken cancellationToken)
    {
        var yearId = request.AcademicYearId
            ?? await _db.AcademicYears.AsNoTracking()
                .Where(x => x.SchoolId == teacher.SchoolId && x.IsCurrent && !x.IsDeleted)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken)
            ?? await _db.AcademicYears.AsNoTracking()
                .Where(x => x.SchoolId == teacher.SchoolId && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        var subjectId = request.SubjectId
            ?? await _db.Subjects.AsNoTracking()
                .Where(x => x.SchoolId == teacher.SchoolId && !x.IsDeleted)
                .OrderBy(x => x.NameAr)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        var existing = await _db.TeacherAssignments
            .Where(x => x.TeacherId == teacher.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var selected = request.ClassSectionIds.Where(x => x > 0).Distinct().ToHashSet();
        foreach (var assignment in existing.Where(x => !selected.Contains(x.ClassSectionId)))
        {
            assignment.IsActive = false;
            assignment.IsDeleted = true;
            assignment.DeletedAt = DateTime.UtcNow;
        }

        if (!yearId.HasValue || !subjectId.HasValue || selected.Count == 0)
        {
            return;
        }

        var validSectionIds = await _db.ClassSections.AsNoTracking()
            .Where(x => selected.Contains(x.Id) && x.SchoolId == teacher.SchoolId && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var sectionId in validSectionIds)
        {
            var current = existing.FirstOrDefault(x => x.ClassSectionId == sectionId && !x.IsDeleted);
            if (current is null)
            {
                _db.TeacherAssignments.Add(new TeacherAssignment
                {
                    SchoolId = teacher.SchoolId,
                    TeacherId = teacher.Id,
                    AcademicYearId = yearId.Value,
                    SubjectId = subjectId.Value,
                    ClassSectionId = sectionId,
                    IsActive = true
                });
            }
            else
            {
                current.IsActive = true;
                current.AcademicYearId = yearId.Value;
                current.SubjectId = subjectId.Value;
            }
        }
    }

    private async Task SyncLessonsAsync(int teacherId, int schoolId, List<int> lessonIds, CancellationToken cancellationToken)
    {
        var selected = lessonIds.Where(x => x > 0).Distinct().ToHashSet();
        var schoolLessons = await _db.Lessons
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted &&
                        (x.TeacherId == teacherId || selected.Contains(x.Id)))
            .ToListAsync(cancellationToken);

        foreach (var lesson in schoolLessons)
        {
            if (selected.Contains(lesson.Id))
            {
                lesson.TeacherId = teacherId;
            }
            else if (lesson.TeacherId == teacherId && selected.Count > 0)
            {
                // Keep existing links unless explicitly managing selection on edit with empty means keep all.
            }
        }

        if (selected.Count == 0)
        {
            return;
        }

        foreach (var lesson in schoolLessons.Where(x => x.TeacherId == teacherId && !selected.Contains(x.Id)))
        {
            lesson.TeacherId = null;
        }
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
