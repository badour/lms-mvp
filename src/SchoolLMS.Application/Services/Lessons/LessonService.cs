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
                ClassNames = (
                    from c in _db.LessonClassSections
                    join s in _db.ClassSections on c.ClassSectionId equals s.Id
                    join g in _db.GradeLevels on s.GradeLevelId equals g.Id
                    where c.LessonId == x.lesson.Id
                    select g.NameAr + " / " + s.NameAr
                ).ToList()
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

    public async Task<LessonDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (lesson is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(lesson.SchoolId)))
        {
            return null;
        }

        var schoolName = await _db.Schools.AsNoTracking()
            .Where(x => x.Id == lesson.SchoolId)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var subjectName = await _db.Subjects.AsNoTracking()
            .Where(x => x.Id == lesson.SubjectId)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var teacherName = lesson.TeacherId == null
            ? null
            : await _db.Teachers.AsNoTracking()
                .Where(x => x.Id == lesson.TeacherId)
                .Select(x => x.FullNameAr)
                .FirstOrDefaultAsync(cancellationToken);

        var classRows = await (
            from c in _db.LessonClassSections.AsNoTracking()
            join s in _db.ClassSections.AsNoTracking() on c.ClassSectionId equals s.Id
            join g in _db.GradeLevels.AsNoTracking() on s.GradeLevelId equals g.Id
            where c.LessonId == lesson.Id
            orderby g.SortOrder, s.NameAr
            select new { s.Id, Name = g.NameAr + " / " + s.NameAr }
        ).ToListAsync(cancellationToken);

        var materials = await _db.LessonResources.AsNoTracking()
            .Where(x => x.LessonId == lesson.Id && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .Select(x => new LessonMaterialDto
            {
                Id = x.Id,
                Title = x.Title,
                OriginalFileName = x.OriginalFileName,
                ContentType = x.ContentType,
                FileSizeBytes = x.FileSizeBytes,
                RelativePath = x.RelativePath
            })
            .ToListAsync(cancellationToken);

        return new LessonDetailsDto
        {
            Id = lesson.Id,
            SchoolId = lesson.SchoolId,
            SchoolNameAr = schoolName,
            SubjectId = lesson.SubjectId,
            SubjectNameAr = subjectName,
            TeacherId = lesson.TeacherId ?? 0,
            TeacherNameAr = teacherName,
            TitleAr = lesson.TitleAr,
            Description = lesson.Description,
            Notes = lesson.Notes,
            LessonDateTime = lesson.LessonDateTime,
            Status = lesson.Status,
            IsPosted = lesson.IsPosted,
            VideoPath = lesson.VideoPath,
            VideoOriginalName = lesson.VideoOriginalName,
            VideoUrl = lesson.VideoUrl,
            ClassSectionIds = classRows.Select(x => x.Id).ToList(),
            IncludedClassNames = classRows.Select(x => x.Name).ToList(),
            Materials = materials,
            CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateLessonRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.LessonsManage) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية إضافة درس.");
        }

        var prepared = await PrepareLessonDataAsync(request, cancellationToken);
        if (!prepared.Succeeded)
        {
            return ServiceResult<int>.Failure(prepared.Errors);
        }

        var data = prepared.Data!;
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
                CourseUnitId = data.CourseUnitId,
                TeacherId = request.TeacherId,
                GradeLevelId = data.GradeLevelId,
                ClassSectionId = data.ClassIds.First(),
                TitleAr = data.SubjectNameAr,
                TitleEn = data.SubjectNameEn,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                TeacherNotes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                Status = data.Status,
                IsPosted = data.IsPosted,
                LessonDateTime = request.LessonDateTime,
                PublishDate = data.IsPosted ? DateTime.UtcNow : null,
                VideoPath = videoPath,
                VideoOriginalName = videoOriginalName,
                SortOrder = await _db.Lessons.CountAsync(x => x.SchoolId == request.SchoolId && x.SubjectId == request.SubjectId && !x.IsDeleted, cancellationToken) + 1
            };

            foreach (var classId in data.ClassIds)
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
                    Classes = data.ClassIds.Count,
                    Materials = lesson.Resources.Count
                },
                schoolId: lesson.SchoolId,
                cancellationToken: cancellationToken);

            return ServiceResult<int>.Success(lesson.Id);
        }
        catch
        {
            await CleanupFilesAsync(videoPath, savedMaterialPaths, cancellationToken);
            throw;
        }
    }

    public async Task<ServiceResult> UpdateAsync(UpdateLessonRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.LessonsManage) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية تعديل الدرس.");
        }

        var lesson = await _db.Lessons
            .Include(x => x.IncludedClasses)
            .Include(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (lesson is null)
        {
            return ServiceResult.Failure("الدرس غير موجود.");
        }

        if (!_currentUser.CanAccessSchool(lesson.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بتعديل هذا الدرس.");
        }

        // Keep school fixed on edit for data integrity.
        request.SchoolId = lesson.SchoolId;

        var prepared = await PrepareLessonDataAsync(request, cancellationToken);
        if (!prepared.Succeeded)
        {
            return ServiceResult.Failure(prepared.Errors);
        }

        var data = prepared.Data!;
        var filesToDelete = new List<string>();
        var newlySaved = new List<string>();

        try
        {
            if (request.RemoveVideo || (request.Video is not null && request.Video.Length > 0))
            {
                if (!string.IsNullOrWhiteSpace(lesson.VideoPath))
                {
                    filesToDelete.Add(lesson.VideoPath);
                }

                lesson.VideoPath = null;
                lesson.VideoOriginalName = null;
            }

            if (request.Video is not null && request.Video.Length > 0)
            {
                var videoOriginalName = Path.GetFileName(request.Video.FileName);
                var videoPath = await _fileStorage.SaveAsync(
                    request.Video.Content,
                    videoOriginalName,
                    $"lessons/{lesson.SchoolId}/videos",
                    cancellationToken);
                newlySaved.Add(videoPath);
                lesson.VideoPath = videoPath;
                lesson.VideoOriginalName = videoOriginalName;
            }

            var removeIds = request.RemoveMaterialIds.Where(x => x > 0).Distinct().ToHashSet();
            foreach (var resource in lesson.Resources.Where(x => !x.IsDeleted && removeIds.Contains(x.Id)).ToList())
            {
                if (!string.IsNullOrWhiteSpace(resource.RelativePath))
                {
                    filesToDelete.Add(resource.RelativePath);
                }

                resource.IsDeleted = true;
                resource.DeletedAt = DateTime.UtcNow;
                resource.DeletedByUserId = _currentUser.UserId;
            }

            var sort = lesson.Resources.Where(x => !x.IsDeleted).Select(x => x.SortOrder).DefaultIfEmpty(0).Max() + 1;
            foreach (var material in request.Materials.Where(x => x.Length > 0))
            {
                var original = Path.GetFileName(material.FileName);
                var path = await _fileStorage.SaveAsync(
                    material.Content,
                    original,
                    $"lessons/{lesson.SchoolId}/materials",
                    cancellationToken);
                newlySaved.Add(path);

                lesson.Resources.Add(new LessonResource
                {
                    SchoolId = lesson.SchoolId,
                    ResourceType = "Document",
                    Title = Path.GetFileNameWithoutExtension(original),
                    RelativePath = path,
                    OriginalFileName = original,
                    ContentType = material.ContentType,
                    FileSizeBytes = material.Length,
                    SortOrder = sort++
                });
            }

            lesson.SubjectId = request.SubjectId;
            lesson.CourseUnitId = data.CourseUnitId;
            lesson.TeacherId = request.TeacherId;
            lesson.GradeLevelId = data.GradeLevelId;
            lesson.ClassSectionId = data.ClassIds.First();
            lesson.TitleAr = data.SubjectNameAr;
            lesson.TitleEn = data.SubjectNameEn;
            lesson.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            lesson.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
            lesson.TeacherNotes = lesson.Notes;
            lesson.Status = data.Status;
            lesson.IsPosted = data.IsPosted;
            lesson.LessonDateTime = request.LessonDateTime;
            if (data.IsPosted && lesson.PublishDate is null)
            {
                lesson.PublishDate = DateTime.UtcNow;
            }

            _db.LessonClassSections.RemoveRange(lesson.IncludedClasses);
            lesson.IncludedClasses.Clear();
            foreach (var classId in data.ClassIds)
            {
                lesson.IncludedClasses.Add(new LessonClassSection
                {
                    LessonId = lesson.Id,
                    ClassSectionId = classId
                });
            }

            await _db.SaveChangesAsync(cancellationToken);

            foreach (var path in filesToDelete.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                await _fileStorage.DeleteAsync(path, cancellationToken);
            }

            await _audit.LogAsync(
                "Lesson.Update",
                nameof(Lesson),
                lesson.Id.ToString(),
                schoolId: lesson.SchoolId,
                cancellationToken: cancellationToken);

            return ServiceResult.Success();
        }
        catch
        {
            await CleanupFilesAsync(null, newlySaved, cancellationToken);
            throw;
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.LessonsManage) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية حذف الدرس.");
        }

        var lesson = await _db.Lessons
            .Include(x => x.IncludedClasses)
            .Include(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (lesson is null)
        {
            return ServiceResult.Failure("الدرس غير موجود.");
        }

        if (!_currentUser.CanAccessSchool(lesson.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بحذف هذا الدرس.");
        }

        var filesToDelete = new List<string>();
        if (!string.IsNullOrWhiteSpace(lesson.VideoPath))
        {
            filesToDelete.Add(lesson.VideoPath);
        }

        foreach (var resource in lesson.Resources.Where(x => !x.IsDeleted))
        {
            if (!string.IsNullOrWhiteSpace(resource.RelativePath))
            {
                filesToDelete.Add(resource.RelativePath);
            }

            resource.IsDeleted = true;
            resource.DeletedAt = DateTime.UtcNow;
            resource.DeletedByUserId = _currentUser.UserId;
        }

        _db.LessonClassSections.RemoveRange(lesson.IncludedClasses);

        lesson.IsDeleted = true;
        lesson.DeletedAt = DateTime.UtcNow;
        lesson.DeletedByUserId = _currentUser.UserId;
        lesson.VideoPath = null;
        lesson.VideoOriginalName = null;

        await _db.SaveChangesAsync(cancellationToken);

        foreach (var path in filesToDelete.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await _fileStorage.DeleteAsync(path, cancellationToken);
        }

        await _audit.LogAsync(
            "Lesson.Delete",
            nameof(Lesson),
            lesson.Id.ToString(),
            schoolId: lesson.SchoolId,
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<LessonFileDto?> GetVideoFileAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        var lesson = await _db.Lessons.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == lessonId && !x.IsDeleted, cancellationToken);

        if (lesson is null || string.IsNullOrWhiteSpace(lesson.VideoPath))
        {
            return null;
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(lesson.SchoolId))
        {
            return null;
        }

        return new LessonFileDto
        {
            LessonId = lesson.Id,
            SchoolId = lesson.SchoolId,
            RelativePath = lesson.VideoPath,
            OriginalFileName = lesson.VideoOriginalName ?? Path.GetFileName(lesson.VideoPath),
            ContentType = GuessContentType(lesson.VideoOriginalName ?? lesson.VideoPath),
            IsVideo = true
        };
    }

    public async Task<LessonFileDto?> GetMaterialFileAsync(int materialId, CancellationToken cancellationToken = default)
    {
        var material = await _db.LessonResources.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == materialId && !x.IsDeleted, cancellationToken);

        if (material is null || string.IsNullOrWhiteSpace(material.RelativePath))
        {
            return null;
        }

        var lesson = await _db.Lessons.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == material.LessonId && !x.IsDeleted, cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(lesson.SchoolId))
        {
            return null;
        }

        return new LessonFileDto
        {
            LessonId = lesson.Id,
            SchoolId = lesson.SchoolId,
            RelativePath = material.RelativePath,
            OriginalFileName = material.OriginalFileName ?? Path.GetFileName(material.RelativePath),
            ContentType = material.ContentType ?? GuessContentType(material.OriginalFileName ?? material.RelativePath),
            IsVideo = false
        };
    }

    private async Task<ServiceResult<PreparedLessonData>> PrepareLessonDataAsync(CreateLessonRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.CanAccessSchool(request.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<PreparedLessonData>.Failure("غير مصرح بالوصول إلى هذه المدرسة.");
        }

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<PreparedLessonData>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var schoolExists = await _db.Schools.AnyAsync(x => x.Id == request.SchoolId && !x.IsDeleted, cancellationToken);
        if (!schoolExists)
        {
            return ServiceResult<PreparedLessonData>.Failure("المدرسة غير موجودة.");
        }

        var subject = await _db.Subjects.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.SubjectId && x.SchoolId == request.SchoolId && !x.IsDeleted, cancellationToken);
        if (subject is null)
        {
            return ServiceResult<PreparedLessonData>.Failure("اسم الدرس / المادة غير موجود لهذه المدرسة.");
        }

        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TeacherId && x.SchoolId == request.SchoolId && x.IsActive && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return ServiceResult<PreparedLessonData>.Failure("المعلم غير موجود لهذه المدرسة.");
        }

        var classIds = request.ClassSectionIds.Where(x => x > 0).Distinct().ToList();
        if (classIds.Count == 0)
        {
            return ServiceResult<PreparedLessonData>.Failure("اختر شعبة واحدةً على الأقل.");
        }

        var classes = await _db.ClassSections.AsNoTracking()
            .Where(x => classIds.Contains(x.Id) && x.SchoolId == request.SchoolId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (classes.Count != classIds.Count)
        {
            return ServiceResult<PreparedLessonData>.Failure("بعض الشعب المحددة غير صالحة لهذه المدرسة.");
        }

        var videoValidation = ValidateUpload(request.Video, VideoExtensions, maxBytes: null, "فيديو الدرس");
        if (videoValidation is not null)
        {
            return ServiceResult<PreparedLessonData>.Failure(videoValidation);
        }

        foreach (var material in request.Materials)
        {
            var materialValidation = ValidateUpload(material, MaterialExtensions, MaxMaterialBytes, "مواد الدرس");
            if (materialValidation is not null)
            {
                return ServiceResult<PreparedLessonData>.Failure(materialValidation);
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

        return ServiceResult<PreparedLessonData>.Success(new PreparedLessonData(
            courseUnit.Id,
            classIds,
            classes.Select(x => (int?)x.GradeLevelId).FirstOrDefault(),
            subject.NameAr,
            subject.NameEn,
            status,
            request.IsPosted || status == PublicationStatus.Published));
    }

    private async Task CleanupFilesAsync(string? videoPath, IEnumerable<string> materialPaths, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(videoPath))
        {
            await _fileStorage.DeleteAsync(videoPath, cancellationToken);
        }

        foreach (var path in materialPaths)
        {
            await _fileStorage.DeleteAsync(path, cancellationToken);
        }
    }

    private static string? ValidateUpload(FileUploadInput? file, HashSet<string> allowedExtensions, long? maxBytes, string label)
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

        if (maxBytes.HasValue && file.Length > maxBytes.Value)
        {
            var maxMb = maxBytes.Value / (1024 * 1024);
            return $"{label}: حجم الملف يتجاوز {maxMb} ميجابايت.";
        }

        return null;
    }

    private static string GuessContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".mov" => "video/quicktime",
            ".m4v" => "video/x-m4v",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".zip" => "application/zip",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    private sealed record PreparedLessonData(
        int CourseUnitId,
        List<int> ClassIds,
        int? GradeLevelId,
        string SubjectNameAr,
        string? SubjectNameEn,
        PublicationStatus Status,
        bool IsPosted);
}
