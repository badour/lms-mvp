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
                SectionName = section != null ? section.NameAr : "أ",
                SchoolId = stage.SchoolId,
                YearName = stage.YearName ?? yearName,
                IsActive = stage.IsActive,
                GradeLevelId = grade != null ? grade.Id : null,
                ClassSectionId = section != null ? section.Id : null
            }
        ).ToListAsync(cancellationToken);

        var distinctStageIds = stages.Select(x => x.Id).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        var activeStageIds = stages.Where(x => x.IsActive && x.Id.HasValue).Select(x => x.Id!.Value).Distinct().ToList();

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
            StageCount = distinctStageIds.Count,
            ActiveStageCount = activeStageIds.Count,
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

        // New school is available in the school list; stages without a pick default to it.
        foreach (var stage in request.Stages ?? [])
        {
            if (stage.SchoolId <= 0)
            {
                stage.SchoolId = school.Id;
            }
        }

        var stageTargetError = await ValidateStageSchoolIdsAsync(request.Stages, cancellationToken);
        if (stageTargetError is not null)
        {
            return ServiceResult<int>.Failure(stageTargetError);
        }

        await EnsureAcademicYearAsync(school.Id, request.YearName, cancellationToken);
        await ApplyStagesBySchoolAsync(school.Id, request.YearName, request.Stages ?? [], replacePrimary: true, cancellationToken);
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

        foreach (var stage in request.Stages ?? [])
        {
            if (stage.SchoolId <= 0)
            {
                stage.SchoolId = request.Id;
            }
        }

        var stageTargetError = await ValidateStageSchoolIdsAsync(request.Stages, cancellationToken);
        if (stageTargetError is not null)
        {
            return ServiceResult.Failure(stageTargetError);
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
        await ApplyStagesBySchoolAsync(school.Id, request.YearName, request.Stages ?? [], replacePrimary: true, cancellationToken);
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

    private async Task<string?> ValidateStageSchoolIdsAsync(List<SchoolStageItemDto>? stages, CancellationToken cancellationToken)
    {
        var ids = (stages ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.StageName))
            .Select(x => x.SchoolId)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return null;
        }

        if (ids.Any(x => x <= 0))
        {
            return "اختر اسم المدرسة من القائمة لكل مرحلة.";
        }

        var validCount = await _db.Schools.AsNoTracking()
            .CountAsync(x => ids.Contains(x.Id) && !x.IsDeleted, cancellationToken);

        return validCount == ids.Count ? null : "إحدى المراحل مرتبطة بمدرسة غير موجودة.";
    }

    private async Task ApplyStagesBySchoolAsync(
        int primarySchoolId,
        string schoolYearName,
        List<SchoolStageItemDto> stages,
        bool replacePrimary,
        CancellationToken cancellationToken)
    {
        var incoming = (stages ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.StageName)
                        && !string.IsNullOrWhiteSpace(x.ClassName)
                        && !string.IsNullOrWhiteSpace(x.SectionName))
            .ToList();

        foreach (var group in incoming.GroupBy(x => x.SchoolId))
        {
            await EnsureAcademicYearAsync(group.Key, schoolYearName, cancellationToken);
            var replaceMissing = replacePrimary && group.Key == primarySchoolId;
            await SyncStagesAsync(group.Key, schoolYearName, group.ToList(), replaceMissing, cancellationToken);
        }

        if (replacePrimary && !incoming.Any(x => x.SchoolId == primarySchoolId))
        {
            await SyncStagesAsync(primarySchoolId, schoolYearName, [], replaceMissing: true, cancellationToken);
        }
    }

    private async Task SyncStagesAsync(
        int schoolId,
        string schoolYearName,
        List<SchoolStageItemDto> stages,
        bool replaceMissing,
        CancellationToken cancellationToken)
    {
        var incoming = (stages ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.StageName)
                        && !string.IsNullOrWhiteSpace(x.ClassName)
                        && !string.IsNullOrWhiteSpace(x.SectionName))
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

        var stagesByName = existingStages
            .GroupBy(x => x.NameAr.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var keepStageIds = new HashSet<int>();
        var keepGradeIds = new HashSet<int>();
        var keepSectionIds = new HashSet<int>();
        var sort = 1;

        foreach (var item in incoming)
        {
            var stageName = item.StageName.Trim();
            var className = item.ClassName.Trim();
            var sectionName = item.SectionName.Trim();
            var yearName = string.IsNullOrWhiteSpace(item.YearName) ? schoolYearName.Trim() : item.YearName.Trim();

            AcademicStage? stage = null;
            if (item.Id.HasValue)
            {
                stage = existingStages.FirstOrDefault(x => x.Id == item.Id.Value);
            }

            if (stage is null)
            {
                stagesByName.TryGetValue(stageName, out stage);
            }

            if (stage is null)
            {
                stage = new AcademicStage { SchoolId = schoolId };
                _db.AcademicStages.Add(stage);
                existingStages.Add(stage);
            }

            stage.NameAr = stageName;
            stage.NameEn = stageName;
            stage.YearName = yearName;
            stage.IsActive = keepStageIds.Contains(stage.Id) ? stage.IsActive || item.IsActive : item.IsActive;
            if (!keepStageIds.Contains(stage.Id))
            {
                stage.SortOrder = sort++;
            }

            stage.IsDeleted = false;
            stage.DeletedAt = null;

            await _db.SaveChangesAsync(cancellationToken);
            keepStageIds.Add(stage.Id);
            stagesByName[stageName] = stage;

            var grade = item.GradeLevelId.HasValue
                ? existingGrades.FirstOrDefault(x => x.Id == item.GradeLevelId.Value)
                : null;

            grade ??= existingGrades.FirstOrDefault(x =>
                x.AcademicStageId == stage.Id &&
                string.Equals(x.NameAr.Trim(), className, StringComparison.OrdinalIgnoreCase));

            if (grade is null)
            {
                grade = new GradeLevel
                {
                    SchoolId = schoolId,
                    AcademicStageId = stage.Id
                };
                _db.GradeLevels.Add(grade);
                existingGrades.Add(grade);
            }

            grade.AcademicStageId = stage.Id;
            grade.NameAr = className;
            grade.NameEn = className;
            grade.SortOrder = keepGradeIds.Contains(grade.Id) ? grade.SortOrder : sort;
            grade.IsActive = keepGradeIds.Contains(grade.Id) ? grade.IsActive || item.IsActive : item.IsActive;
            grade.IsDeleted = false;
            grade.DeletedAt = null;
            await _db.SaveChangesAsync(cancellationToken);
            keepGradeIds.Add(grade.Id);

            var section = item.ClassSectionId.HasValue
                ? existingSections.FirstOrDefault(x => x.Id == item.ClassSectionId.Value)
                : null;

            section ??= existingSections.FirstOrDefault(x =>
                x.GradeLevelId == grade.Id &&
                string.Equals(x.NameAr.Trim(), sectionName, StringComparison.OrdinalIgnoreCase));

            if (section is null)
            {
                section = new ClassSection
                {
                    SchoolId = schoolId,
                    GradeLevelId = grade.Id,
                    Capacity = 30
                };
                _db.ClassSections.Add(section);
                existingSections.Add(section);
            }

            section.GradeLevelId = grade.Id;
            section.NameAr = sectionName;
            section.NameEn = sectionName;
            section.IsActive = item.IsActive;
            section.IsDeleted = false;
            section.DeletedAt = null;
            await _db.SaveChangesAsync(cancellationToken);
            keepSectionIds.Add(section.Id);
        }

        if (!replaceMissing)
        {
            return;
        }

        var now = DateTime.UtcNow;
        foreach (var section in existingSections.Where(x => x.Id > 0 && !keepSectionIds.Contains(x.Id)))
        {
            section.IsDeleted = true;
            section.DeletedAt = now;
            section.IsActive = false;
        }

        foreach (var grade in existingGrades.Where(x => x.Id > 0 && !keepGradeIds.Contains(x.Id)))
        {
            grade.IsDeleted = true;
            grade.DeletedAt = now;
            grade.IsActive = false;
        }

        foreach (var stage in existingStages.Where(x => x.Id > 0 && !keepStageIds.Contains(x.Id)))
        {
            stage.IsDeleted = true;
            stage.DeletedAt = now;
            stage.IsActive = false;
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
