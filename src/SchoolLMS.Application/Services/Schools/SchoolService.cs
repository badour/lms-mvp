using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Schools;
using SchoolLMS.Domain.Entities.Tenancy;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Schools;

public class SchoolService : ISchoolService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateSchoolRequest> _createValidator;

    public SchoolService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IAuditService audit,
        IValidator<CreateSchoolRequest> createValidator)
    {
        _db = db;
        _currentUser = currentUser;
        _audit = audit;
        _createValidator = createValidator;
    }

    public async Task<IReadOnlyList<SchoolListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _db.Schools.AsNoTracking().Where(x => !x.IsDeleted);

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.Id));
        }

        return await query
            .OrderBy(x => x.NameAr)
            .Select(x => new SchoolListItemDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                Phone = x.Phone,
                Email = x.Email,
                SchoolType = x.SchoolType,
                IsActive = x.IsActive,
                BranchCount = _db.SchoolBranches.Count(b => b.SchoolId == x.Id && !b.IsDeleted),
                StudentCount = _db.Students.Count(s => s.SchoolId == x.Id && !s.IsDeleted)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SchoolDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.CanAccessSchool(id) && !_currentUser.IsSuperAdmin)
        {
            return null;
        }

        return await _db.Schools.AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new SchoolDetailsDto
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                LogoPath = x.LogoPath,
                Address = x.Address,
                Phone = x.Phone,
                SecondaryPhone = x.SecondaryPhone,
                Email = x.Email,
                SchoolType = x.SchoolType,
                GenderType = x.GenderType,
                Currency = x.Currency,
                IsActive = x.IsActive,
                BranchCount = _db.SchoolBranches.Count(b => b.SchoolId == x.Id && !b.IsDeleted),
                StudentCount = _db.Students.Count(s => s.SchoolId == x.Id && !s.IsDeleted)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateSchoolRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsSuperAdmin && !_currentUser.HasPermission(PermissionNames.SchoolsCreate))
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية إنشاء مدرسة.");
        }

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var school = new School
        {
            NameAr = request.NameAr.Trim(),
            NameEn = request.NameEn.Trim(),
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            SchoolType = request.SchoolType,
            GenderType = request.GenderType,
            Currency = request.Currency,
            IsActive = true,
            Settings = new SchoolSettings()
        };

        _db.Schools.Add(school);
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("School.Create", nameof(School), school.Id.ToString(),
            newValues: request, schoolId: school.Id, cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(school.Id);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateSchoolRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.CanAccessSchool(request.Id) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بالوصول إلى هذه المدرسة.");
        }

        if (!_currentUser.HasPermission(PermissionNames.SchoolsEdit) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية تعديل المدرسة.");
        }

        var school = await _db.Schools.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (school is null)
        {
            return ServiceResult.Failure("المدرسة غير موجودة.");
        }

        var old = new { school.NameAr, school.NameEn, school.IsActive };

        school.NameAr = request.NameAr.Trim();
        school.NameEn = request.NameEn.Trim();
        school.Address = request.Address;
        school.Phone = request.Phone;
        school.Email = request.Email;
        school.SchoolType = request.SchoolType;
        school.GenderType = request.GenderType;
        school.Currency = request.Currency;
        school.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("School.Update", nameof(School), school.Id.ToString(),
            oldValues: old, newValues: request, schoolId: school.Id, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
