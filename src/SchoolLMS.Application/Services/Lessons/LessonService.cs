using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Lessons;
using SchoolLMS.Domain.Entities.Lms;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Lessons;

public class LessonService : ILessonService
{
    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".webm", ".mov", ".m4v"
    };

    private static readonly HashSet<string> MaterialExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".png", ".jpg", ".jpeg", ".zip"
    };

    private const long MaxVideoBytes = 200 * 1024 * 1024;
    private const long MaxMaterialBytes = 20 * 1024 * 1024;

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IFileStorage _fileStorage;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateLessonRequest> _createValidator;

    public LessonService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IFileStorage fileStorage,
        IAuditService audit,
        IValidator<CreateLessonRequest> createValidator)
    {
        _db = db;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _audit = audit;
        _createValidator = createValidator;
    }

    public async Task<PagedResult<LessonListItemDto>> SearchAsync(LessonSearchRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query =
            from lesson in _db.Lessons.AsNoTracking()
            where !lesson.IsDeleted
            join school in _db.Schools.AsNoTracking() on lesson.SchoolId equals school.Id
            join subject in _db.Subjects.AsNoTracking() on lesson.SubjectId equals subject.Id
            select new { lesson, school, subject };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.lesson.SchoolId));
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(x => x.lesson.SchoolId == request.SchoolId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(x => x.lesson.SubjectId == request.SubjectId.Value);
        }

        if (request.TeacherId.HasValue)
        {
            query = query.Where(x => x.lesson.TeacherId == request.TeacherId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.lesson.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.lesson.TitleAr.Contains(term) ||
                x.subject.NameAr.Contains(term) ||
                (x.lesson.Description != null && x.lesson.Description.Contains(term)) ||
                (x.lesson.Notes != null && x.lesson.Notes.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var pageItems = await query
            .OrderByDescending(x => x.lesson.LessonDateTime)
            .ThenByDescending(x => x.lesson.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.lesson.Id,
                x.lesson.SchoolId,
                SchoolNameAr = x.school.NameAr,
                x.lesson.TitleAr,
                SubjectNameAr = x.subject.NameAr,
                TeacherNameAr = x.lesson.TeacherId == null
                    ? null
                    : _db.Teachers.Where(t => t.Id == x.lesson.TeacherId).Select(t => t.FullNameAr).FirstOrDefault(),
                x.lesson.LessonDateTime,
                x.lesson.Status,
                x.lesson.IsPosted,
                HasVideo = x.lesson.VideoPath != null || x.lesson.VideoUrl != null,
                MaterialCount = _db.LessonResources.Count(r => r.LessonId == x.lesson.Id && !r.IsDeleted),
                x.lesson.CreatedAt,
                ClassNames = _db.LessonClassSections
                    .Where(c => c.LessonId == x.lesson.Id)
                    .Join(_db.ClassSections, c => c.ClassSectionId, s => s.Id, (c, s) => s.NameAr)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var items = pageItems.Select(x => new LessonListItemDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            SchoolNameAr = x.SchoolNameAr,
            TitleAr = x.TitleAr,
            SubjectNameAr = x.SubjectNameAr,
            TeacherNameAr = x.TeacherNameAr,
            IncludedClassesText = x.ClassNames.Count == 0 ? "—" : string.Join("، ", x.ClassNames),
            LessonDateTime = x.LessonDateTime,
            Status = x.Status,
            IsPosted = x.IsPosted,
            HasVideo = x.HasVideo,
            MaterialCount = x.MaterialCount,
            CreatedAt = x.CreatedAt
        }).ToList();

        return new PagedResult<LessonListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.LessonsManage) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية إضافة درس.");
        }

        if (!_currentUser.CanAccessSchool(request.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("غير مصرح بالإضافة إلى هذه المدرسة.");
        }

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var schoolExists = await _db.Schools.AnyAsync(x => x.Id == request.SchoolId && !x.IsDeleted, cancellationToken);
        if (!schoolExists)
        {
            return ServiceResult<int>.Failure("المدرسة غير موجودة.");
        }

        var subject = await _db.Subjects.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.SubjectId && x.SchoolId == request.SchoolId && !x.IsDeleted, cancellationToken);
        if (subject is null)
        {
            return ServiceResult<int>.Failure("اسم الدرس / المادة غير موجود لهذه المدرسة.");
        }

        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TeacherId && x.SchoolId == request.SchoolId && x.IsActive && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return ServiceResult<int>.Failure("المعلم غير موجود لهذه المدرسة.");
        }

        var classIds = request.ClassSectionIds.Where(x => x > 0).Distinct().ToList();
        if (classIds.Count == 0)
        {
            return ServiceResult<int>.Failure("اختر صفاً واحداً على الأقل.");
        }

        var classes = await _db.ClassSections.AsNoTracking()
            .Where(x => classIds.Contains(x.Id) && x.SchoolId == request.SchoolId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (classes.Count != classIds.Count)
        {
            return ServiceResult<int>.Failure("بعض الصفوف المحددة غير صالحة لهذه المدرسة.");
        }

        var videoValidation = ValidateUpload(request.Video, VideoExtensions, MaxVideoBytes, "فيديو الدرس");
        if (videoValidation is not null)
        {
            return ServiceResult<int>.Failure(videoValidation);
        }

        foreach (var material in request.Materials)
        {
            var materialValidation = ValidateUpload(material, MaterialExtensions, MaxMaterialBytes, "مواد الدرس");
            if (materialValidation is not null)
            {
                return ServiceResult<int>.Failure(materialValidation);
            }
        }

        var courseUnit = await _db.CourseUnits
            .FirstOrDefaultAsync(x =>
                x.SchoolId == request.SchoolId &&
                x.SubjectId == request.SubjectId &&
                !x.IsDeleted &&
                x.IsActive, cancellationToken);

        if (courseUnit is null)
        {
            courseUnit = new CourseUnit
            {
                SchoolId = request.SchoolId,
                SubjectId = request.SubjectId,
                TitleAr = $"وحدة {subject.NameAr}",
                TitleEn = $"{subject.NameEn} Unit",
                SortOrder = 1,
                IsActive = true
            };
            _db.CourseUnits.Add(courseUnit);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var status = request.IsPosted && request.Status == PublicationStatus.Draft
            ? PublicationStatus.Published
            : request.Status;

        string? videoPath = null;
        string? videoOriginalName = null;
        var savedMaterialPaths = new List<string>();

        try
        {
            if (request.Video is not null && request.Video.Length > 0)
            {
                videoOriginalName = Path.GetFileName(request.Video.FileName);
                videoPath = await _fileStorage.SaveAsync(
                    request.Video.Content,
                    videoOriginalName,
                    $"lessons/{request.SchoolId}/videos",
                    cancellationToken);
            }

            var lesson = new Lesson
            {
                SchoolId = request.SchoolId,
                SubjectId = request.SubjectId,
                CourseUnitId = courseUnit.Id,
                TeacherId = request.TeacherId,
                GradeLevelId = classes.Select(x => (int?)x.GradeLevelId).FirstOrDefault(),
                ClassSectionId = classIds.First(),
                TitleAr = subject.NameAr,
                TitleEn = subject.NameEn,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                TeacherNotes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                Status = status,
                IsPosted = request.IsPosted || status == PublicationStatus.Published,
                LessonDateTime = request.LessonDateTime,
                PublishDate = (request.IsPosted || status == PublicationStatus.Published) ? DateTime.UtcNow : null,
                VideoPath = videoPath,
                VideoOriginalName = videoOriginalName,
                SortOrder = await _db.Lessons.CountAsync(x => x.SchoolId == request.SchoolId && x.SubjectId == request.SubjectId && !x.IsDeleted, cancellationToken) + 1
            };

            foreach (var classId in classIds)
            {
                lesson.IncludedClasses.Add(new LessonClassSection { ClassSectionId = classId });
            }

            var sort = 1;
            foreach (var material in request.Materials.Where(x => x.Length > 0))
            {
                var original = Path.GetFileName(material.FileName);
                var path = await _fileStorage.SaveAsync(
                    material.Content,
                    original,
                    $"lessons/{request.SchoolId}/materials",
                    cancellationToken);
                savedMaterialPaths.Add(path);

                lesson.Resources.Add(new LessonResource
                {
                    SchoolId = request.SchoolId,
                    ResourceType = "Document",
                    Title = Path.GetFileNameWithoutExtension(original),
                    RelativePath = path,
                    OriginalFileName = original,
                    ContentType = material.ContentType,
                    FileSizeBytes = material.Length,
                    SortOrder = sort++
                });
            }

            _db.Lessons.Add(lesson);
            await _db.SaveChangesAsync(cancellationToken);

            await _audit.LogAsync(
                "Lesson.Create",
                nameof(Lesson),
                lesson.Id.ToString(),
                newValues: new
                {
                    lesson.SchoolId,
                    lesson.SubjectId,
                    lesson.TeacherId,
                    lesson.TitleAr,
                    lesson.Status,
                    lesson.IsPosted,
                    Classes = classIds.Count,
                    Materials = lesson.Resources.Count
                },
                schoolId: lesson.SchoolId,
                cancellationToken: cancellationToken);

            return ServiceResult<int>.Success(lesson.Id);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(videoPath))
            {
                await _fileStorage.DeleteAsync(videoPath, cancellationToken);
            }

            foreach (var path in savedMaterialPaths)
            {
                await _fileStorage.DeleteAsync(path, cancellationToken);
            }

            throw;
        }
    }

    private static string? ValidateUpload(FileUploadInput? file, HashSet<string> allowedExtensions, long maxBytes, string label)
    {
        if (file is null || file.Length <= 0)
        {
            return null;
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
        {
            return $"{label}: نوع الملف غير مدعوم.";
        }

        if (file.Length > maxBytes)
        {
            var maxMb = maxBytes / (1024 * 1024);
            return $"{label}: حجم الملف يتجاوز {maxMb} ميجابايت.";
        }

        return null;
    }
}
