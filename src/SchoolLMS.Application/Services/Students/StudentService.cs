using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Entities.People;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Students;

public class StudentService : IStudentService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateStudentRequest> _createValidator;

    public StudentService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IAuditService audit,
        IValidator<CreateStudentRequest> createValidator)
    {
        _db = db;
        _currentUser = currentUser;
        _audit = audit;
        _createValidator = createValidator;
    }

    public async Task<PagedResult<StudentListItemDto>> SearchAsync(StudentSearchRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query = from s in _db.Students.AsNoTracking()
                    where !s.IsDeleted
                    join school in _db.Schools.AsNoTracking() on s.SchoolId equals school.Id
                    select new { s, school };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.s.SchoolId));
        }

        if (request.SchoolId.HasValue)
        {
            query = query.Where(x => x.s.SchoolId == request.SchoolId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.s.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.s.FullNameAr.Contains(term) ||
                (x.s.FullNameEn != null && x.s.FullNameEn.Contains(term)) ||
                x.s.StudentNumber.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.s.FullNameAr)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new StudentListItemDto
            {
                Id = x.s.Id,
                SchoolId = x.s.SchoolId,
                SchoolNameAr = x.school.NameAr,
                StudentNumber = x.s.StudentNumber,
                FullNameAr = x.s.FullNameAr,
                FullNameEn = x.s.FullNameEn,
                Gender = x.s.Gender,
                Status = x.s.Status,
                GradeNameAr = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.s.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => e.GradeLevel!.NameAr)
                    .FirstOrDefault(),
                SectionNameAr = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.s.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => e.ClassSection!.NameAr)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<StudentListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<StudentDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students.AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new StudentDetailsDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                SchoolBranchId = x.SchoolBranchId,
                StudentNumber = x.StudentNumber,
                FullNameAr = x.FullNameAr,
                FullNameEn = x.FullNameEn,
                ProfileImagePath = x.ProfileImagePath,
                Gender = x.Gender,
                DateOfBirth = x.DateOfBirth,
                PlaceOfBirth = x.PlaceOfBirth,
                Nationality = x.Nationality,
                NationalId = x.NationalId,
                RegistrationDate = x.RegistrationDate,
                Status = x.Status,
                PreviousSchool = x.PreviousSchool,
                Notes = x.Notes,
                SchoolNameAr = _db.Schools.Where(s => s.Id == x.SchoolId).Select(s => s.NameAr).FirstOrDefault() ?? string.Empty
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(student.SchoolId)))
        {
            return null;
        }

        return student;
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.CanAccessSchool(request.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("غير مصرح بالإضافة إلى هذه المدرسة.");
        }

        if (!_currentUser.HasPermission(PermissionNames.StudentsCreate) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية إنشاء طالب.");
        }

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var exists = await _db.Students.AnyAsync(x =>
            x.SchoolId == request.SchoolId &&
            x.StudentNumber == request.StudentNumber &&
            !x.IsDeleted, cancellationToken);

        if (exists)
        {
            return ServiceResult<int>.Failure("رقم الطالب مستخدم مسبقاً في هذه المدرسة.");
        }

        var student = new Student
        {
            SchoolId = request.SchoolId,
            SchoolBranchId = request.SchoolBranchId,
            StudentNumber = request.StudentNumber.Trim(),
            FullNameAr = request.FullNameAr.Trim(),
            FullNameEn = request.FullNameEn?.Trim(),
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            PlaceOfBirth = request.PlaceOfBirth,
            Nationality = request.Nationality,
            NationalId = request.NationalId,
            PreviousSchool = request.PreviousSchool,
            Notes = request.Notes,
            RegistrationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = StudentStatus.Active
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync(cancellationToken);

        if (request.AcademicYearId.HasValue && request.GradeLevelId.HasValue && request.ClassSectionId.HasValue)
        {
            _db.StudentEnrollments.Add(new StudentEnrollment
            {
                SchoolId = request.SchoolId,
                StudentId = student.Id,
                AcademicYearId = request.AcademicYearId.Value,
                GradeLevelId = request.GradeLevelId.Value,
                ClassSectionId = request.ClassSectionId.Value,
                Status = EnrollmentStatus.Active,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });
            await _db.SaveChangesAsync(cancellationToken);
        }

        await _audit.LogAsync("Student.Create", nameof(Student), student.Id.ToString(),
            newValues: request, schoolId: student.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(student.Id);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (student is null)
        {
            return ServiceResult.Failure("الطالب غير موجود.");
        }

        if (!_currentUser.CanAccessSchool(student.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بتعديل هذا الطالب.");
        }

        if (!_currentUser.HasPermission(PermissionNames.StudentsEdit) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية تعديل الطالب.");
        }

        var duplicate = await _db.Students.AnyAsync(x =>
            x.Id != request.Id &&
            x.SchoolId == student.SchoolId &&
            x.StudentNumber == request.StudentNumber &&
            !x.IsDeleted, cancellationToken);

        if (duplicate)
        {
            return ServiceResult.Failure("رقم الطالب مستخدم مسبقاً في هذه المدرسة.");
        }

        student.StudentNumber = request.StudentNumber.Trim();
        student.FullNameAr = request.FullNameAr.Trim();
        student.FullNameEn = request.FullNameEn?.Trim();
        student.Gender = request.Gender;
        student.DateOfBirth = request.DateOfBirth;
        student.PlaceOfBirth = request.PlaceOfBirth;
        student.Nationality = request.Nationality;
        student.NationalId = request.NationalId;
        student.PreviousSchool = request.PreviousSchool;
        student.Notes = request.Notes;
        student.Status = request.Status;
        student.SchoolBranchId = request.SchoolBranchId;

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Student.Update", nameof(Student), student.Id.ToString(),
            newValues: request, schoolId: student.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
