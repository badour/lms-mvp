using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Schools;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Entities.Tenancy;
using SchoolLMS.Domain.Enums;
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
                Address = x.Address,
                SchoolType = x.SchoolType,
                GenderType = x.GenderType,
                Currency = x.Currency,
                YearName = _db.AcademicYears
                    .Where(y => y.SchoolId == x.Id && !y.IsDeleted && y.IsCurrent)
                    .Select(y => y.NameAr)
                    .FirstOrDefault(),
                IsActive = x.IsActive,
                BranchCount = _db.SchoolBranches.Count(b => b.SchoolId == x.Id && !b.IsDeleted),
                StudentCount = _db.Students.Count(s => s.SchoolId == x.Id && !s.IsDeleted),
                StageCount = _db.AcademicStages.Count(s => s.SchoolId == x.Id && !s.IsDeleted),
                ActiveStageCount = _db.AcademicStages.Count(s => s.SchoolId == x.Id && !s.IsDeleted && s.IsActive)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SchoolDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.CanAccessSchool(id) && !_currentUser.IsSuperAdmin)
        {
            return null;
        }

        var school = await _db.Schools.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (school is null)
        {
            return null;
        }

        var yearName = await _db.AcademicYears.AsNoTracking()
            .Where(y => y.SchoolId == id && !y.IsDeleted && y.IsCurrent)
            .Select(y => y.NameAr)
            .FirstOrDefaultAsync(cancellationToken);

        var stages = await (
            from stage in _db.AcademicStages.AsNoTracking()
            where stage.SchoolId == id && !stage.IsDeleted
            join grade in _db.GradeLevels.AsNoTracking() on stage.Id equals grade.AcademicStageId into gg
            from grade in gg.Where(g => !g.IsDeleted).DefaultIfEmpty()
            join section in _db.ClassSections.AsNoTracking() on grade.Id equals section.GradeLevelId into sg
            from section in sg.Where(s => !s.IsDeleted).DefaultIfEmpty()
            orderby stage.SortOrder, grade.SortOrder, section.NameAr
            select new SchoolStageItemDto
            {
                Id = stage.Id,
                StageName = stage.NameAr,
                ClassName = grade != null ? grade.NameAr : string.Empty,
                YearName = stage.YearName ?? yearName,
                IsActive = stage.IsActive,
                GradeLevelId = grade != null ? grade.Id : null,
                ClassSectionId = section != null ? section.Id : null
            }
        ).ToListAsync(cancellationToken);

        // Collapse duplicate stage rows if multiple sections — keep first grade/section per stage for edit form simplicity
        stages = stages
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .ToList();

        return new SchoolDetailsDto
        {
            Id = school.Id,
            NameAr = school.NameAr,
            NameEn = school.NameEn,
            LogoPath = school.LogoPath,
            Address = school.Address,
            Phone = school.Phone,
            SecondaryPhone = school.SecondaryPhone,
            Email = school.Email,
            SchoolType = school.SchoolType,
            GenderType = school.GenderType,
            Currency = school.Currency,
            YearName = yearName,
            IsActive = school.IsActive,
            BranchCount = await _db.SchoolBranches.CountAsync(b => b.SchoolId == id && !b.IsDeleted, cancellationToken),
            StudentCount = await _db.Students.CountAsync(s => s.SchoolId == id && !s.IsDeleted, cancellationToken),
            StageCount = stages.Count,
            ActiveStageCount = stages.Count(x => x.IsActive),
            Stages = stages
        };
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
            Address = request.Address.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email.Trim(),
            SchoolType = request.SchoolType.Trim(),
            GenderType = request.GenderType.Trim(),
            Currency = request.Currency.Trim(),
            IsActive = true,
            Settings = new SchoolSettings()
        };

        _db.Schools.Add(school);
        await _db.SaveChangesAsync(cancellationToken);

        await EnsureAcademicYearAsync(school.Id, request.YearName, cancellationToken);
        await SyncStagesAsync(school.Id, request.YearName, request.Stages, cancellationToken);
        await EnsureDefaultSubjectsAsync(school.Id, cancellationToken);
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

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var school = await _db.Schools.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (school is null)
        {
            return ServiceResult.Failure("المدرسة غير موجودة.");
        }

        school.NameAr = request.NameAr.Trim();
        school.NameEn = request.NameEn.Trim();
        school.Address = request.Address.Trim();
        school.Phone = request.Phone?.Trim();
        school.Email = request.Email.Trim();
        school.SchoolType = request.SchoolType.Trim();
        school.GenderType = request.GenderType.Trim();
        school.Currency = request.Currency.Trim();
        school.IsActive = request.IsActive;

        await EnsureAcademicYearAsync(school.Id, request.YearName, cancellationToken);
        await SyncStagesAsync(school.Id, request.YearName, request.Stages, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("School.Update", nameof(School), school.Id.ToString(),
            newValues: request, schoolId: school.Id, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsSuperAdmin && !_currentUser.HasPermission(PermissionNames.SchoolsDelete))
        {
            return ServiceResult.Failure("ليس لديك صلاحية حذف المدرسة.");
        }

        if (!_currentUser.CanAccessSchool(id) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بحذف هذه المدرسة.");
        }

        var school = await _db.Schools.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (school is null)
        {
            return ServiceResult.Failure("المدرسة غير موجودة.");
        }

        var now = DateTime.UtcNow;
        school.IsDeleted = true;
        school.DeletedAt = now;
        school.IsActive = false;

        var stages = await _db.AcademicStages.Where(x => x.SchoolId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var stage in stages)
        {
            stage.IsDeleted = true;
            stage.DeletedAt = now;
            stage.IsActive = false;
        }

        var grades = await _db.GradeLevels.Where(x => x.SchoolId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var grade in grades)
        {
            grade.IsDeleted = true;
            grade.DeletedAt = now;
            grade.IsActive = false;
        }

        var sections = await _db.ClassSections.Where(x => x.SchoolId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        var sectionIds = sections.Select(x => x.Id).ToList();
        foreach (var section in sections)
        {
            section.IsDeleted = true;
            section.DeletedAt = now;
            section.IsActive = false;
        }

        if (sectionIds.Count > 0)
        {
            var schedules = await _db.ClassSchedules
                .Where(x => sectionIds.Contains(x.ClassSectionId) && !x.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var schedule in schedules)
            {
                schedule.IsDeleted = true;
                schedule.DeletedAt = now;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("School.Delete", nameof(School), school.Id.ToString(),
            schoolId: school.Id, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    private async Task EnsureAcademicYearAsync(int schoolId, string yearName, CancellationToken cancellationToken)
    {
        var name = yearName.Trim();
        var years = await _db.AcademicYears.Where(x => x.SchoolId == schoolId && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var y in years)
        {
            y.IsCurrent = false;
        }

        var current = years.FirstOrDefault(x => string.Equals(x.NameAr, name, StringComparison.OrdinalIgnoreCase));
        if (current is null)
        {
            current = new AcademicYear
            {
                SchoolId = schoolId,
                NameAr = name,
                NameEn = name,
                StartDate = new DateOnly(DateTime.Today.Year, 9, 1),
                EndDate = new DateOnly(DateTime.Today.Year + 1, 6, 30),
                Status = AcademicYearStatus.Open,
                IsCurrent = true
            };
            _db.AcademicYears.Add(current);
        }
        else
        {
            current.IsCurrent = true;
            current.Status = AcademicYearStatus.Open;
        }
    }

    private async Task SyncStagesAsync(int schoolId, string schoolYearName, List<SchoolStageItemDto> stages, CancellationToken cancellationToken)
    {
        var incoming = (stages ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.StageName) && !string.IsNullOrWhiteSpace(x.ClassName))
            .ToList();

        var existingStages = await _db.AcademicStages
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        var existingGrades = await _db.GradeLevels
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        var existingSections = await _db.ClassSections
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var keepStageIds = new HashSet<int>();
        var sort = 1;
        foreach (var item in incoming)
        {
            AcademicStage stage;
            if (item.Id.HasValue)
            {
                stage = existingStages.FirstOrDefault(x => x.Id == item.Id.Value)
                        ?? new AcademicStage { SchoolId = schoolId };
                if (stage.Id == 0)
                {
                    _db.AcademicStages.Add(stage);
                }
            }
            else
            {
                stage = new AcademicStage { SchoolId = schoolId };
                _db.AcademicStages.Add(stage);
            }

            stage.NameAr = item.StageName.Trim();
            stage.NameEn = item.StageName.Trim();
            stage.YearName = string.IsNullOrWhiteSpace(item.YearName) ? schoolYearName.Trim() : item.YearName.Trim();
            stage.IsActive = item.IsActive;
            stage.SortOrder = sort++;
            stage.IsDeleted = false;
            stage.DeletedAt = null;

            await _db.SaveChangesAsync(cancellationToken);
            keepStageIds.Add(stage.Id);

            var grade = item.GradeLevelId.HasValue
                ? existingGrades.FirstOrDefault(x => x.Id == item.GradeLevelId.Value)
                : existingGrades.FirstOrDefault(x => x.AcademicStageId == stage.Id);

            if (grade is null)
            {
                grade = new GradeLevel
                {
                    SchoolId = schoolId,
                    AcademicStageId = stage.Id
                };
                _db.GradeLevels.Add(grade);
            }

            grade.AcademicStageId = stage.Id;
            grade.NameAr = item.ClassName.Trim();
            grade.NameEn = item.ClassName.Trim();
            grade.SortOrder = sort;
            grade.IsActive = item.IsActive;
            grade.IsDeleted = false;
            grade.DeletedAt = null;
            await _db.SaveChangesAsync(cancellationToken);

            var section = item.ClassSectionId.HasValue
                ? existingSections.FirstOrDefault(x => x.Id == item.ClassSectionId.Value)
                : existingSections.FirstOrDefault(x => x.GradeLevelId == grade.Id);

            if (section is null)
            {
                section = new ClassSection
                {
                    SchoolId = schoolId,
                    GradeLevelId = grade.Id,
                    NameAr = "أ",
                    NameEn = "A",
                    Capacity = 30
                };
                _db.ClassSections.Add(section);
            }

            section.GradeLevelId = grade.Id;
            section.IsActive = item.IsActive;
            section.IsDeleted = false;
            section.DeletedAt = null;
        }

        foreach (var stage in existingStages.Where(x => !keepStageIds.Contains(x.Id)))
        {
            stage.IsDeleted = true;
            stage.DeletedAt = DateTime.UtcNow;
            stage.IsActive = false;

            foreach (var grade in existingGrades.Where(g => g.AcademicStageId == stage.Id))
            {
                grade.IsDeleted = true;
                grade.DeletedAt = DateTime.UtcNow;
                grade.IsActive = false;
                foreach (var section in existingSections.Where(s => s.GradeLevelId == grade.Id))
                {
                    section.IsDeleted = true;
                    section.DeletedAt = DateTime.UtcNow;
                    section.IsActive = false;
                }
            }
        }
    }

    private async Task EnsureDefaultSubjectsAsync(int schoolId, CancellationToken cancellationToken)
    {
        var hasSubjects = await _db.Subjects.AnyAsync(x => x.SchoolId == schoolId && !x.IsDeleted, cancellationToken);
        if (hasSubjects)
        {
            return;
        }

        foreach (var name in new[] { "رياضيات", "لغة عربية", "علوم", "لغة إنجليزية" })
        {
            _db.Subjects.Add(new Subject
            {
                SchoolId = schoolId,
                NameAr = name,
                NameEn = name
            });
        }
    }
}
