using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Attendance;
using SchoolLMS.Domain.Entities.Operations;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Attendance;

public class AttendanceAdminService : IAttendanceAdminService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAuditService _audit;
    private readonly IValidator<SaveAttendanceRequest> _validator;

    public AttendanceAdminService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IAuditService audit,
        IValidator<SaveAttendanceRequest> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _audit = audit;
        _validator = validator;
    }

    public async Task<IReadOnlyList<AttendanceSessionListItemDto>> ListAsync(
        AttendanceSessionFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query =
            from session in _db.AttendanceSessions.AsNoTracking()
            where !session.IsDeleted
            join school in _db.Schools.AsNoTracking() on session.SchoolId equals school.Id
            join teacher in _db.Teachers.AsNoTracking() on session.TeacherId equals teacher.Id into tg
            from teacher in tg.DefaultIfEmpty()
            join section in _db.ClassSections.AsNoTracking() on session.ClassSectionId equals section.Id
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            select new { session, school, teacher, section, grade, stage };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.session.SchoolId));
        }

        if (filter.SchoolId.HasValue)
        {
            query = query.Where(x => x.session.SchoolId == filter.SchoolId.Value);
        }

        if (filter.TeacherId.HasValue)
        {
            query = query.Where(x => x.session.TeacherId == filter.TeacherId.Value);
        }

        if (filter.AttendanceDate.HasValue)
        {
            query = query.Where(x => x.session.AttendanceDate == filter.AttendanceDate.Value);
        }

        return await query
            .OrderByDescending(x => x.session.AttendanceDate)
            .ThenByDescending(x => x.session.Id)
            .Select(x => new AttendanceSessionListItemDto
            {
                Id = x.session.Id,
                SchoolId = x.session.SchoolId,
                SchoolNameAr = x.school.NameAr,
                TeacherId = x.session.TeacherId,
                TeacherNameAr = x.teacher != null ? x.teacher.FullNameAr : "—",
                StageNameAr = x.stage.NameAr,
                SectionNameAr = x.grade.NameAr + " / " + x.section.NameAr,
                AttendanceDate = x.session.AttendanceDate,
                PresentCount = _db.StudentAttendances.Count(r =>
                    r.AttendanceSessionId == x.session.Id && !r.IsDeleted && r.Status == AttendanceStatus.Present),
                AbsentCount = _db.StudentAttendances.Count(r =>
                    r.AttendanceSessionId == x.session.Id && !r.IsDeleted && r.Status == AttendanceStatus.Absent),
                Status = x.session.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AttendanceSessionDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var meta = await (
            from session in _db.AttendanceSessions.AsNoTracking()
            where session.Id == id && !session.IsDeleted
            join school in _db.Schools.AsNoTracking() on session.SchoolId equals school.Id
            join teacher in _db.Teachers.AsNoTracking() on session.TeacherId equals teacher.Id into tg
            from teacher in tg.DefaultIfEmpty()
            join section in _db.ClassSections.AsNoTracking() on session.ClassSectionId equals section.Id
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            join stage in _db.AcademicStages.AsNoTracking() on grade.AcademicStageId equals stage.Id
            select new
            {
                session.Id,
                session.SchoolId,
                SchoolNameAr = school.NameAr,
                TeacherId = session.TeacherId ?? 0,
                TeacherNameAr = teacher != null ? teacher.FullNameAr : "—",
                AcademicStageId = stage.Id,
                StageNameAr = stage.NameAr,
                ClassSectionId = section.Id,
                SectionNameAr = grade.NameAr + " / " + section.NameAr,
                session.AttendanceDate,
                session.Status
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (meta is null)
        {
            return null;
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(meta.SchoolId))
        {
            return null;
        }

        var students = await (
            from record in _db.StudentAttendances.AsNoTracking()
            join student in _db.Students.AsNoTracking() on record.StudentId equals student.Id
            where record.AttendanceSessionId == id && !record.IsDeleted && !student.IsDeleted
            orderby student.FullNameAr
            select new AttendanceStudentRowDto
            {
                StudentId = student.Id,
                StudentNumber = student.StudentNumber,
                FullNameAr = student.FullNameAr,
                IsPresent = record.Status == AttendanceStatus.Present
            }
        ).ToListAsync(cancellationToken);

        return new AttendanceSessionDetailDto
        {
            Id = meta.Id,
            SchoolId = meta.SchoolId,
            SchoolNameAr = meta.SchoolNameAr,
            TeacherId = meta.TeacherId,
            TeacherNameAr = meta.TeacherNameAr,
            AcademicStageId = meta.AcademicStageId,
            StageNameAr = meta.StageNameAr,
            ClassSectionId = meta.ClassSectionId,
            SectionNameAr = meta.SectionNameAr,
            AttendanceDate = meta.AttendanceDate,
            Status = meta.Status,
            PresentCount = students.Count(x => x.IsPresent),
            AbsentCount = students.Count(x => !x.IsPresent),
            Students = students
        };
    }

    public async Task<IReadOnlyList<AttendanceStudentRowDto>> GetRosterAsync(
        AttendanceRosterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.TeacherId <= 0 || request.ClassSectionId <= 0)
        {
            return Array.Empty<AttendanceStudentRowDto>();
        }

        var section = await _db.ClassSections.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ClassSectionId && !x.IsDeleted, cancellationToken);
        if (section is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(section.SchoolId)))
        {
            return Array.Empty<AttendanceStudentRowDto>();
        }

        var teacherOk = await _db.Teachers.AsNoTracking()
            .AnyAsync(x => x.Id == request.TeacherId && x.SchoolId == section.SchoolId && !x.IsDeleted, cancellationToken);
        if (!teacherOk)
        {
            return Array.Empty<AttendanceStudentRowDto>();
        }

        var stageOk = await (
            from s in _db.ClassSections.AsNoTracking()
            join g in _db.GradeLevels.AsNoTracking() on s.GradeLevelId equals g.Id
            where s.Id == request.ClassSectionId && g.AcademicStageId == request.AcademicStageId
            select s.Id
        ).AnyAsync(cancellationToken);
        if (!stageOk)
        {
            return Array.Empty<AttendanceStudentRowDto>();
        }

        var students = await (
            from enrollment in _db.StudentEnrollments.AsNoTracking()
            join student in _db.Students.AsNoTracking() on enrollment.StudentId equals student.Id
            where enrollment.ClassSectionId == request.ClassSectionId
                  && !enrollment.IsDeleted
                  && enrollment.Status == EnrollmentStatus.Active
                  && !student.IsDeleted
            orderby student.FullNameAr
            select new AttendanceStudentRowDto
            {
                StudentId = student.Id,
                StudentNumber = student.StudentNumber,
                FullNameAr = student.FullNameAr,
                IsPresent = true
            }
        ).ToListAsync(cancellationToken);

        if (request.AttendanceDate.HasValue)
        {
            var sessionId = await _db.AttendanceSessions.AsNoTracking()
                .Where(x => x.ClassSectionId == request.ClassSectionId
                            && x.TeacherId == request.TeacherId
                            && x.AttendanceDate == request.AttendanceDate.Value
                            && !x.IsDeleted)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (sessionId.HasValue)
            {
                var marks = await _db.StudentAttendances.AsNoTracking()
                    .Where(x => x.AttendanceSessionId == sessionId.Value && !x.IsDeleted)
                    .Select(x => new { x.StudentId, x.Status })
                    .ToListAsync(cancellationToken);
                var map = marks.ToDictionary(x => x.StudentId, x => x.Status == AttendanceStatus.Present);
                foreach (var row in students)
                {
                    if (map.TryGetValue(row.StudentId, out var present))
                    {
                        row.IsPresent = present;
                    }
                }
            }
        }

        return students;
    }

    public async Task<ServiceResult<int>> SaveAsync(SaveAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.AttendanceRecord) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult<int>.Failure("ليس لديك صلاحية تسجيل الحضور.");
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(request.SchoolId))
        {
            return ServiceResult<int>.Failure("غير مصرح بهذه المدرسة.");
        }

        var section = await _db.ClassSections.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ClassSectionId
                                      && x.SchoolId == request.SchoolId
                                      && !x.IsDeleted, cancellationToken);
        if (section is null)
        {
            return ServiceResult<int>.Failure("الشعبة غير موجودة لهذه المدرسة.");
        }

        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TeacherId
                                      && x.SchoolId == request.SchoolId
                                      && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return ServiceResult<int>.Failure("المعلم غير موجود في هذه المدرسة.");
        }

        var stageMatches = await (
            from s in _db.ClassSections.AsNoTracking()
            join g in _db.GradeLevels.AsNoTracking() on s.GradeLevelId equals g.Id
            where s.Id == request.ClassSectionId && g.AcademicStageId == request.AcademicStageId
            select s.Id
        ).AnyAsync(cancellationToken);
        if (!stageMatches)
        {
            return ServiceResult<int>.Failure("الشعبة لا تتبع المرحلة المحددة.");
        }

        var rosterIds = await _db.StudentEnrollments.AsNoTracking()
            .Where(x => x.ClassSectionId == request.ClassSectionId
                        && !x.IsDeleted
                        && x.Status == EnrollmentStatus.Active)
            .Select(x => x.StudentId)
            .ToListAsync(cancellationToken);

        if (rosterIds.Count == 0)
        {
            return ServiceResult<int>.Failure("لا يوجد طلاب مسجلون في هذه الشعبة.");
        }

        var marks = request.Students
            .Where(x => rosterIds.Contains(x.StudentId))
            .GroupBy(x => x.StudentId)
            .ToDictionary(g => g.Key, g => g.Last().IsPresent);

        foreach (var studentId in rosterIds.Where(id => !marks.ContainsKey(id)))
        {
            marks[studentId] = true;
        }

        AttendanceSession? session = null;
        if (request.SessionId.HasValue)
        {
            session = await _db.AttendanceSessions
                .Include(x => x.Records)
                .FirstOrDefaultAsync(x => x.Id == request.SessionId.Value && !x.IsDeleted, cancellationToken);
            if (session is null)
            {
                return ServiceResult<int>.Failure("سجل الحضور غير موجود.");
            }

            if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(session.SchoolId))
            {
                return ServiceResult<int>.Failure("غير مصرح بتعديل هذا السجل.");
            }
        }
        else
        {
            session = await _db.AttendanceSessions
                .Include(x => x.Records)
                .FirstOrDefaultAsync(x =>
                    x.ClassSectionId == request.ClassSectionId
                    && x.TeacherId == request.TeacherId
                    && x.AttendanceDate == request.AttendanceDate
                    && !x.IsDeleted, cancellationToken);
        }

        if (session is null)
        {
            session = new AttendanceSession
            {
                SchoolId = request.SchoolId,
                ClassSectionId = request.ClassSectionId,
                TeacherId = request.TeacherId,
                AttendanceDate = request.AttendanceDate,
                Status = PublicationStatus.Published
            };
            _db.AttendanceSessions.Add(session);
        }
        else
        {
            session.SchoolId = request.SchoolId;
            session.ClassSectionId = request.ClassSectionId;
            session.TeacherId = request.TeacherId;
            session.AttendanceDate = request.AttendanceDate;
            session.Status = PublicationStatus.Published;
        }

        foreach (var studentId in rosterIds)
        {
            var present = marks[studentId];
            var record = session.Records.FirstOrDefault(x => x.StudentId == studentId && !x.IsDeleted);
            if (record is null)
            {
                session.Records.Add(new StudentAttendance
                {
                    SchoolId = request.SchoolId,
                    StudentId = studentId,
                    Status = present ? AttendanceStatus.Present : AttendanceStatus.Absent
                });
            }
            else
            {
                record.Status = present ? AttendanceStatus.Present : AttendanceStatus.Absent;
                record.IsDeleted = false;
            }
        }

        foreach (var leftover in session.Records.Where(x => !x.IsDeleted && !rosterIds.Contains(x.StudentId)))
        {
            leftover.IsDeleted = true;
            leftover.DeletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Attendance.Save", nameof(AttendanceSession), session.Id.ToString(),
            schoolId: session.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(session.Id);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission(PermissionNames.AttendanceRecord)
            && !_currentUser.HasPermission(PermissionNames.AttendanceEdit)
            && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية حذف الحضور.");
        }

        var session = await _db.AttendanceSessions
            .Include(x => x.Records)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (session is null)
        {
            return ServiceResult.Failure("سجل الحضور غير موجود.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(session.SchoolId))
        {
            return ServiceResult.Failure("غير مصرح بحذف هذا السجل.");
        }

        session.IsDeleted = true;
        session.DeletedAt = DateTime.UtcNow;
        foreach (var record in session.Records.Where(x => !x.IsDeleted))
        {
            record.IsDeleted = true;
            record.DeletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Attendance.Delete", nameof(AttendanceSession), session.Id.ToString(),
            schoolId: session.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
