using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Schedules;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Schedules;

public class ScheduleAdminService : IScheduleAdminService
{
    public static readonly string[] DayNamesAr =
    [
        "الأحد", "الإثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت"
    ];

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAuditService _audit;
    private readonly IValidator<SaveScheduleRequest> _validator;

    public ScheduleAdminService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IAuditService audit,
        IValidator<SaveScheduleRequest> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _audit = audit;
        _validator = validator;
    }

    public async Task<IReadOnlyList<ScheduleListItemDto>> ListAsync(int? schoolId, CancellationToken cancellationToken = default)
    {
        var sectionIdsWithSchedule = await _db.ClassSchedules.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.ClassSectionId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var query =
            from section in _db.ClassSections.AsNoTracking()
            where !section.IsDeleted && sectionIdsWithSchedule.Contains(section.Id)
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            join school in _db.Schools.AsNoTracking() on section.SchoolId equals school.Id
            select new { section, grade, stage, school };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.section.SchoolId));
        }

        if (schoolId.HasValue)
        {
            query = query.Where(x => x.section.SchoolId == schoolId.Value);
        }

        return await query
            .OrderBy(x => x.school.NameAr)
            .ThenBy(x => x.stage.SortOrder)
            .ThenBy(x => x.grade.SortOrder)
            .ThenBy(x => x.section.NameAr)
            .Select(x => new ScheduleListItemDto
            {
                ClassSectionId = x.section.Id,
                SchoolId = x.school.Id,
                SchoolNameAr = x.school.NameAr,
                StageNameAr = x.stage.NameAr,
                GradeNameAr = x.grade.NameAr,
                SectionNameAr = x.section.NameAr,
                YearNameAr = x.stage.YearName
                    ?? _db.AcademicYears.Where(y => y.SchoolId == x.school.Id && y.IsCurrent && !y.IsDeleted)
                        .Select(y => y.NameAr).FirstOrDefault()
                    ?? "—",
                FilledSlots = _db.ClassSchedules.Count(s => s.ClassSectionId == x.section.Id && !s.IsDeleted),
                UpdatedAt = _db.ClassSchedules
                    .Where(s => s.ClassSectionId == x.section.Id && !s.IsDeleted)
                    .Max(s => (DateTime?)s.UpdatedAt)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ScheduleDetailsDto?> GetBySectionAsync(int classSectionId, CancellationToken cancellationToken = default)
    {
        var meta = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            join school in _db.Schools.AsNoTracking() on section.SchoolId equals school.Id
            where section.Id == classSectionId && !section.IsDeleted
            select new
            {
                section.Id,
                section.SchoolId,
                StageId = stage.Id,
                SchoolName = school.NameAr,
                StageName = stage.NameAr,
                GradeName = grade.NameAr,
                SectionName = section.NameAr
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (meta is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(meta.SchoolId)))
        {
            return null;
        }

        var periods = await EnsurePeriodsAsync(meta.SchoolId, cancellationToken);
        var cells = await _db.ClassSchedules.AsNoTracking()
            .Where(x => x.ClassSectionId == classSectionId && !x.IsDeleted)
            .Select(x => new
            {
                x.DayOfWeek,
                PeriodSort = _db.TeachingPeriods.Where(p => p.Id == x.TeachingPeriodId).Select(p => p.SortOrder).FirstOrDefault(),
                x.SubjectId,
                x.TeacherId,
                x.EntryText
            })
            .ToListAsync(cancellationToken);

        return new ScheduleDetailsDto
        {
            SchoolId = meta.SchoolId,
            AcademicStageId = meta.StageId,
            ClassSectionId = meta.Id,
            SchoolNameAr = meta.SchoolName,
            StageNameAr = meta.StageName,
            GradeNameAr = meta.GradeName,
            SectionNameAr = meta.SectionName,
            PeriodNames = periods.Select(p => p.NameAr).ToList(),
            Cells = cells.Select(c => new ScheduleCellDto
            {
                DayOfWeek = c.DayOfWeek,
                PeriodSortOrder = c.PeriodSort,
                SubjectId = c.SubjectId,
                TeacherId = c.TeacherId,
                EntryText = c.EntryText
            }).ToList()
        };
    }

    public async Task<ServiceResult> SaveAsync(SaveScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(request.SchoolId))
        {
            return ServiceResult.Failure("غير مصرح بهذه المدرسة.");
        }

        var sectionOk = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.Id == request.ClassSectionId
                  && section.SchoolId == request.SchoolId
                  && !section.IsDeleted
                  && grade.AcademicStageId == request.AcademicStageId
                  && grade.IsActive
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            where stage.IsActive && !stage.IsDeleted
            select section.Id
        ).AnyAsync(cancellationToken);

        if (!sectionOk)
        {
            return ServiceResult.Failure("الشعبة أو المرحلة غير صالحة أو غير نشطة.");
        }

        var yearId = await _db.AcademicYears.AsNoTracking()
            .Where(x => x.SchoolId == request.SchoolId && x.IsCurrent && !x.IsDeleted)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? await _db.AcademicYears.AsNoTracking()
                .Where(x => x.SchoolId == request.SchoolId && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        if (!yearId.HasValue)
        {
            return ServiceResult.Failure("لا توجد سنة دراسية للمدرسة.");
        }

        var periods = await EnsurePeriodsAsync(request.SchoolId, cancellationToken);
        var periodBySort = periods.ToDictionary(x => x.SortOrder, x => x.Id);

        var existing = await _db.ClassSchedules
            .Where(x => x.ClassSectionId == request.ClassSectionId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var row in existing)
        {
            row.IsDeleted = true;
            row.DeletedAt = DateTime.UtcNow;
        }

        foreach (var cell in request.Cells ?? [])
        {
            if (cell.DayOfWeek is < 0 or > 6)
            {
                continue;
            }

            if (!periodBySort.TryGetValue(cell.PeriodSortOrder, out var periodId))
            {
                continue;
            }

            var text = cell.EntryText?.Trim();
            if (string.IsNullOrWhiteSpace(text) && !cell.SubjectId.HasValue && !cell.TeacherId.HasValue)
            {
                continue;
            }

            _db.ClassSchedules.Add(new ClassSchedule
            {
                SchoolId = request.SchoolId,
                AcademicYearId = yearId.Value,
                ClassSectionId = request.ClassSectionId,
                TeachingPeriodId = periodId,
                DayOfWeek = cell.DayOfWeek,
                SubjectId = cell.SubjectId,
                TeacherId = cell.TeacherId,
                EntryText = text
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Schedule.Save", nameof(ClassSchedule), request.ClassSectionId.ToString(),
            schoolId: request.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteBySectionAsync(int classSectionId, CancellationToken cancellationToken = default)
    {
        var section = await _db.ClassSections.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == classSectionId && !x.IsDeleted, cancellationToken);
        if (section is null)
        {
            return ServiceResult.Failure("الشعبة غير موجودة.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(section.SchoolId))
        {
            return ServiceResult.Failure("غير مصرح بحذف هذا الجدول.");
        }

        var rows = await _db.ClassSchedules
            .Where(x => x.ClassSectionId == classSectionId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        if (rows.Count == 0)
        {
            return ServiceResult.Failure("لا يوجد جدول محفوظ لهذه الشعبة.");
        }

        foreach (var row in rows)
        {
            row.IsDeleted = true;
            row.DeletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Schedule.Delete", nameof(ClassSchedule), classSectionId.ToString(),
            schoolId: section.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    private async Task<List<TeachingPeriod>> EnsurePeriodsAsync(int schoolId, CancellationToken cancellationToken)
    {
        var periods = await _db.TeachingPeriods
            .Where(x => x.SchoolId == schoolId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        if (periods.Count >= 8)
        {
            return periods.Take(8).ToList();
        }

        var startHour = 8;
        for (var i = periods.Count + 1; i <= 8; i++)
        {
            var start = new TimeOnly(startHour + (i - 1), 0);
            var end = start.AddMinutes(45);
            var period = new TeachingPeriod
            {
                SchoolId = schoolId,
                NameAr = $"الحصة {i}",
                NameEn = $"Period {i}",
                StartTime = start,
                EndTime = end,
                SortOrder = i,
                IsBreak = false
            };
            _db.TeachingPeriods.Add(period);
            periods.Add(period);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return periods.OrderBy(x => x.SortOrder).Take(8).ToList();
    }
}
