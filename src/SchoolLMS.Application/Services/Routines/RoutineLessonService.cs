using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Routines;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Routines;

public class RoutineLessonService : IRoutineLessonService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateRoutineLessonRequest> _validator;

    public RoutineLessonService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IAuditService audit,
        IValidator<CreateRoutineLessonRequest> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _audit = audit;
        _validator = validator;
    }

    public async Task<IReadOnlyList<RoutineLessonListItemDto>> ListAsync(int? schoolId, CancellationToken cancellationToken = default)
    {
        var query =
            from lesson in _db.RoutineLessons.AsNoTracking()
            where !lesson.IsDeleted
            join school in _db.Schools.AsNoTracking() on lesson.SchoolId equals school.Id
            join teacher in _db.Teachers.AsNoTracking() on lesson.TeacherId equals teacher.Id
            join stage in _db.AcademicStages.AsNoTracking() on lesson.AcademicStageId equals stage.Id
            select new { lesson, school, teacher, stage };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.lesson.SchoolId));
        }

        if (schoolId.HasValue)
        {
            query = query.Where(x => x.lesson.SchoolId == schoolId.Value);
        }

        return await query
            .OrderBy(x => x.school.NameAr)
            .ThenBy(x => x.stage.SortOrder)
            .ThenBy(x => x.teacher.FullNameAr)
            .Select(x => new RoutineLessonListItemDto
            {
                Id = x.lesson.Id,
                SchoolId = x.school.Id,
                SchoolNameAr = x.school.NameAr,
                TeacherNameAr = x.teacher.FullNameAr,
                StageNameAr = x.stage.NameAr,
                SessionsPerYear = x.lesson.SessionsPerYear
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateRoutineLessonRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(request.SchoolId))
        {
            return ServiceResult<int>.Failure("غير مصرح بهذه المدرسة.");
        }

        var teacherOk = await _db.Teachers.AsNoTracking().AnyAsync(
            x => x.Id == request.TeacherId && x.SchoolId == request.SchoolId && !x.IsDeleted && x.IsActive,
            cancellationToken);
        if (!teacherOk)
        {
            return ServiceResult<int>.Failure("المعلم غير مرتبط بالمدرسة المختارة.");
        }

        var stageOk = await _db.AcademicStages.AsNoTracking().AnyAsync(
            x => x.Id == request.AcademicStageId && x.SchoolId == request.SchoolId && !x.IsDeleted && x.IsActive,
            cancellationToken);
        if (!stageOk)
        {
            return ServiceResult<int>.Failure("المرحلة غير مرتبطة بالمدرسة المختارة أو غير نشطة.");
        }

        var entity = new RoutineLesson
        {
            SchoolId = request.SchoolId,
            TeacherId = request.TeacherId,
            AcademicStageId = request.AcademicStageId,
            SessionsPerYear = request.SessionsPerYear
        };

        _db.RoutineLessons.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("RoutineLesson.Create", nameof(RoutineLesson), entity.Id.ToString(),
            schoolId: entity.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(entity.Id);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.RoutineLessons.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return ServiceResult.Failure("الدرس غير موجود.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(entity.SchoolId))
        {
            return ServiceResult.Failure("غير مصرح بحذف هذا الدرس.");
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("RoutineLesson.Delete", nameof(RoutineLesson), entity.Id.ToString(),
            schoolId: entity.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
