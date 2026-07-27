using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Common;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Domain.Entities.People;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Students;

public class QimamCertificateService : IQimamCertificateService
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private static readonly HashSet<string> DocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx"
    };

    private const long MaxImageBytes = 5 * 1024 * 1024;
    private const long MaxDocumentBytes = 10 * 1024 * 1024;

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IFileStorage _fileStorage;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateQimamCertificateRequest> _createValidator;

    public QimamCertificateService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IFileStorage fileStorage,
        IAuditService audit,
        IValidator<CreateQimamCertificateRequest> createValidator)
    {
        _db = db;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _audit = audit;
        _createValidator = createValidator;
    }

    public async Task<PagedResult<QimamCertificateListItemDto>> SearchAsync(QimamCertificateSearchRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query = BuildListQuery();

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.SchoolId));
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(x => x.SchoolId == request.SchoolId.Value);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(x => x.StudentId == request.StudentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.CertificateName.Contains(term) ||
                x.ClassName.Contains(term) ||
                x.StudentNameAr.Contains(term) ||
                x.StudentNumber.Contains(term) ||
                (x.Notes != null && x.Notes.Contains(term)) ||
                (x.Description != null && x.Description.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CertificateDate)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<QimamCertificateListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<IReadOnlyList<QimamCertificateListItemDto>> GetForCurrentStudentAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return Array.Empty<QimamCertificateListItemDto>();
        }

        var studentId = await _db.Students.AsNoTracking()
            .Where(x => x.UserId == _currentUser.UserId && !x.IsDeleted)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId is null)
        {
            return Array.Empty<QimamCertificateListItemDto>();
        }

        return await BuildListQuery()
            .Where(x => x.StudentId == studentId.Value)
            .OrderByDescending(x => x.CertificateDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<QimamCertificateListItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await BuildListQuery().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(item.SchoolId)))
        {
            return null;
        }

        return item;
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateQimamCertificateRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.StudentsEdit) &&
            !_currentUser.HasPermission(PermissionNames.StudentsCreate) &&
            !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية إضافة شهادات قمم.");
        }

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var student = await _db.Students
            .FirstOrDefaultAsync(x => x.Id == request.StudentId && !x.IsDeleted, cancellationToken);

        if (student is null)
        {
            return ServiceResult<int>.Failure("الطالب غير موجود.");
        }

        if (!_currentUser.CanAccessSchool(student.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("غير مصرح بالإضافة إلى هذه المدرسة.");
        }

        var imageValidation = ValidateUpload(request.Image, ImageExtensions, MaxImageBytes, "صورة الشهادة");
        if (imageValidation is not null)
        {
            return ServiceResult<int>.Failure(imageValidation);
        }

        var documentValidation = ValidateUpload(request.Document, DocumentExtensions, MaxDocumentBytes, "مستند الشهادة");
        if (documentValidation is not null)
        {
            return ServiceResult<int>.Failure(documentValidation);
        }

        string? imagePath = null;
        string? imageOriginalName = null;
        string? documentPath = null;
        string? documentOriginalName = null;

        try
        {
            if (request.Image is not null && request.Image.Length > 0)
            {
                imageOriginalName = Path.GetFileName(request.Image.FileName);
                imagePath = await _fileStorage.SaveAsync(
                    request.Image.Content,
                    imageOriginalName,
                    $"qimam/{student.SchoolId}/{student.Id}/images",
                    cancellationToken);
            }

            if (request.Document is not null && request.Document.Length > 0)
            {
                documentOriginalName = Path.GetFileName(request.Document.FileName);
                documentPath = await _fileStorage.SaveAsync(
                    request.Document.Content,
                    documentOriginalName,
                    $"qimam/{student.SchoolId}/{student.Id}/documents",
                    cancellationToken);
            }

            var entity = new StudentQimamCertificate
            {
                SchoolId = student.SchoolId,
                StudentId = student.Id,
                CertificateName = request.CertificateName.Trim(),
                CertificateDate = request.CertificateDate!.Value,
                ClassName = request.ClassName.Trim(),
                ImagePath = imagePath,
                ImageOriginalName = imageOriginalName,
                DocumentPath = documentPath,
                DocumentOriginalName = documentOriginalName,
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
            };

            _db.StudentQimamCertificates.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);

            await _audit.LogAsync(
                "QimamCertificate.Create",
                nameof(StudentQimamCertificate),
                entity.Id.ToString(),
                newValues: new
                {
                    entity.StudentId,
                    entity.CertificateName,
                    entity.CertificateDate,
                    entity.ClassName
                },
                schoolId: entity.SchoolId,
                cancellationToken: cancellationToken);

            return ServiceResult<int>.Success(entity.Id);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                await _fileStorage.DeleteAsync(imagePath, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(documentPath))
            {
                await _fileStorage.DeleteAsync(documentPath, cancellationToken);
            }

            throw;
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.StudentsDelete) &&
            !_currentUser.HasPermission(PermissionNames.StudentsEdit) &&
            !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية حذف شهادات قمم.");
        }

        var entity = await _db.StudentQimamCertificates
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity is null)
        {
            return ServiceResult.Failure("الشهادة غير موجودة.");
        }

        if (!_currentUser.CanAccessSchool(entity.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بحذف هذه الشهادة.");
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedByUserId = _currentUser.UserId;
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync(
            "QimamCertificate.Delete",
            nameof(StudentQimamCertificate),
            entity.Id.ToString(),
            schoolId: entity.SchoolId,
            cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<QimamCertificateFileDto?> GetFileAsync(int id, string kind, CancellationToken cancellationToken = default)
    {
        var cert = await _db.StudentQimamCertificates.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (cert is null)
        {
            return null;
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(cert.SchoolId))
        {
            // Allow the owning student to download their own files.
            var owns = !string.IsNullOrWhiteSpace(_currentUser.UserId) &&
                       await _db.Students.AsNoTracking().AnyAsync(
                           x => x.Id == cert.StudentId && x.UserId == _currentUser.UserId && !x.IsDeleted,
                           cancellationToken);
            if (!owns)
            {
                return null;
            }
        }

        var isImage = string.Equals(kind, "image", StringComparison.OrdinalIgnoreCase);
        var path = isImage ? cert.ImagePath : cert.DocumentPath;
        var original = isImage ? cert.ImageOriginalName : cert.DocumentOriginalName;

        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(original))
        {
            return null;
        }

        return new QimamCertificateFileDto
        {
            CertificateId = cert.Id,
            StudentId = cert.StudentId,
            SchoolId = cert.SchoolId,
            RelativePath = path,
            OriginalFileName = original,
            ContentType = GuessContentType(original),
            IsImage = isImage
        };
    }

    private IQueryable<QimamCertificateListItemDto> BuildListQuery()
    {
        return from c in _db.StudentQimamCertificates.AsNoTracking()
               where !c.IsDeleted
               join s in _db.Students.AsNoTracking() on c.StudentId equals s.Id
               join school in _db.Schools.AsNoTracking() on c.SchoolId equals school.Id
               select new QimamCertificateListItemDto
               {
                   Id = c.Id,
                   StudentId = c.StudentId,
                   SchoolId = c.SchoolId,
                   StudentNameAr = s.FullNameAr,
                   StudentNumber = s.StudentNumber,
                   SchoolNameAr = school.NameAr,
                   CertificateName = c.CertificateName,
                   CertificateDate = c.CertificateDate,
                   ClassName = c.ClassName,
                   ImagePath = c.ImagePath,
                   ImageOriginalName = c.ImageOriginalName,
                   DocumentPath = c.DocumentPath,
                   DocumentOriginalName = c.DocumentOriginalName,
                   Notes = c.Notes,
                   Description = c.Description,
                   CreatedAt = c.CreatedAt
               };
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

    private static string GuessContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}
