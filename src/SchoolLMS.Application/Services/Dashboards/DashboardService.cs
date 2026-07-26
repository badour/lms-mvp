using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Dashboards;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Dashboards;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public DashboardService(IApplicationDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync(int? schoolId = null, CancellationToken cancellationToken = default)
    {
        var schoolFilter = ResolveSchoolFilter(schoolId);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var studentsQuery = _db.Students.AsNoTracking().Where(x => !x.IsDeleted);
        var teachersQuery = _db.Teachers.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        var attendanceQuery = _db.StudentAttendances.AsNoTracking()
            .Where(x => !x.IsDeleted && x.AttendanceSession!.AttendanceDate == today);

        if (schoolFilter is not null)
        {
            studentsQuery = studentsQuery.Where(x => schoolFilter.Contains(x.SchoolId));
            teachersQuery = teachersQuery.Where(x => schoolFilter.Contains(x.SchoolId));
            attendanceQuery = attendanceQuery.Where(x => schoolFilter.Contains(x.SchoolId));
        }

        var feesQuery = _db.StudentFees.AsNoTracking().Where(x => !x.IsDeleted);
        var paymentsQuery = _db.Payments.AsNoTracking().Where(x => !x.IsDeleted && x.Status == PaymentStatus.Paid);
        if (schoolFilter is not null)
        {
            feesQuery = feesQuery.Where(x => schoolFilter.Contains(x.SchoolId));
            paymentsQuery = paymentsQuery.Where(x => schoolFilter.Contains(x.SchoolId));
        }

        var announcements = await _db.Announcements.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPublished && (x.ExpireAt == null || x.ExpireAt > DateTime.UtcNow))
            .Where(x => schoolFilter == null || x.SchoolId == null || schoolFilter.Contains(x.SchoolId.Value))
            .OrderByDescending(x => x.IsPinned)
            .ThenByDescending(x => x.PublishAt)
            .Take(5)
            .Select(x => new AnnouncementCardDto
            {
                Id = x.Id,
                TitleAr = x.TitleAr,
                Priority = x.Priority.ToString(),
                PublishAt = x.PublishAt
            })
            .ToListAsync(cancellationToken);

        var studentsPerSchool = await _db.Students.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => schoolFilter == null || schoolFilter.Contains(x.SchoolId))
            .GroupBy(x => x.SchoolId)
            .Select(g => new ChartPointDto
            {
                Label = _db.Schools.Where(s => s.Id == g.Key).Select(s => s.NameAr).FirstOrDefault() ?? g.Key.ToString(),
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return new AdminDashboardDto
        {
            TotalSchools = schoolFilter is null
                ? await _db.Schools.CountAsync(x => !x.IsDeleted && x.IsActive, cancellationToken)
                : schoolFilter.Count,
            TotalStudents = await studentsQuery.CountAsync(cancellationToken),
            TotalTeachers = await teachersQuery.CountAsync(cancellationToken),
            ActiveUsers = await _db.UserSchoolAssignments.AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .Where(x => schoolFilter == null || schoolFilter.Contains(x.SchoolId))
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync(cancellationToken),
            PresentToday = await attendanceQuery.CountAsync(x => x.Status == AttendanceStatus.Present, cancellationToken),
            AbsentToday = await attendanceQuery.CountAsync(x => x.Status == AttendanceStatus.Absent || x.Status == AttendanceStatus.ExcusedAbsence, cancellationToken),
            LateToday = await attendanceQuery.CountAsync(x => x.Status == AttendanceStatus.Late, cancellationToken),
            CollectedFees = await paymentsQuery.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m,
            OutstandingFees = await feesQuery.SumAsync(x => (decimal?)x.RemainingAmount, cancellationToken) ?? 0m,
            PendingHomework = await _db.Assignments.AsNoTracking()
                .Where(x => !x.IsDeleted && x.Status == AssignmentStatus.Published)
                .Where(x => schoolFilter == null || schoolFilter.Contains(x.SchoolId))
                .CountAsync(cancellationToken),
            Announcements = announcements,
            StudentsPerSchool = studentsPerSchool
        };
    }

    public async Task<StudentDashboardDto?> GetStudentDashboardAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted, cancellationToken);
        if (student is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(student.SchoolId)))
        {
            return null;
        }

        var enrollment = await _db.StudentEnrollments.AsNoTracking()
            .Where(x => x.StudentId == studentId && x.Status == EnrollmentStatus.Active && !x.IsDeleted)
            .Select(x => new
            {
                Grade = x.GradeLevel!.NameAr,
                Section = x.ClassSection!.NameAr,
                Year = x.AcademicYear!.NameAr
            })
            .FirstOrDefaultAsync(cancellationToken);

        var attendance = await _db.StudentAttendances.AsNoTracking()
            .Where(x => x.StudentId == studentId && !x.IsDeleted)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Present = g.Count(x => x.Status == AttendanceStatus.Present),
                Absent = g.Count(x => x.Status == AttendanceStatus.Absent || x.Status == AttendanceStatus.ExcusedAbsence),
                Late = g.Count(x => x.Status == AttendanceStatus.Late),
                Total = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var pendingHomework = await _db.Assignments.AsNoTracking()
            .Where(x => !x.IsDeleted && x.Status == AssignmentStatus.Published && x.SchoolId == student.SchoolId)
            .Where(x => !_db.AssignmentSubmissions.Any(s => s.AssignmentId == x.Id && s.StudentId == studentId && !s.IsDeleted && s.SubmittedAt != null))
            .CountAsync(cancellationToken);

        var submittedHomework = await _db.AssignmentSubmissions.AsNoTracking()
            .CountAsync(x => x.StudentId == studentId && !x.IsDeleted && x.SubmittedAt != null, cancellationToken);

        var exams = await _db.Exams.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPublished && x.SchoolId == student.SchoolId && x.ExamDate >= DateOnly.FromDateTime(DateTime.UtcNow))
            .OrderBy(x => x.ExamDate)
            .Take(5)
            .Select(x => new SimpleItemDto
            {
                Title = _db.Subjects.Where(s => s.Id == x.SubjectId).Select(s => s.NameAr).FirstOrDefault() ?? "امتحان",
                Subtitle = x.ExamType.ToString(),
                Date = x.ExamDate.ToDateTime(TimeOnly.MinValue)
            })
            .ToListAsync(cancellationToken);

        var announcements = await _db.Announcements.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPublished && (x.SchoolId == null || x.SchoolId == student.SchoolId))
            .OrderByDescending(x => x.PublishAt)
            .Take(5)
            .Select(x => new AnnouncementCardDto
            {
                Id = x.Id,
                TitleAr = x.TitleAr,
                Priority = x.Priority.ToString(),
                PublishAt = x.PublishAt
            })
            .ToListAsync(cancellationToken);

        var total = attendance?.Total ?? 0;
        var present = attendance?.Present ?? 0;

        return new StudentDashboardDto
        {
            StudentNameAr = student.FullNameAr,
            ProfileImagePath = student.ProfileImagePath,
            SchoolNameAr = await _db.Schools.Where(x => x.Id == student.SchoolId).Select(x => x.NameAr).FirstAsync(cancellationToken),
            GradeNameAr = enrollment?.Grade,
            SectionNameAr = enrollment?.Section,
            StudentNumber = student.StudentNumber,
            AcademicYearNameAr = enrollment?.Year,
            AttendancePercent = total == 0 ? 100 : Math.Round(present * 100m / total, 1),
            DaysPresent = present,
            DaysAbsent = attendance?.Absent ?? 0,
            DaysLate = attendance?.Late ?? 0,
            PendingHomework = pendingHomework,
            SubmittedHomework = submittedHomework,
            Announcements = announcements,
            UpcomingExams = exams
        };
    }

    public async Task<TeacherDashboardDto?> GetTeacherDashboardAsync(int teacherId, CancellationToken cancellationToken = default)
    {
        var teacher = await _db.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == teacherId && !x.IsDeleted, cancellationToken);
        if (teacher is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(teacher.SchoolId)))
        {
            return null;
        }

        var today = (byte)DateTime.UtcNow.DayOfWeek;
        var schedule = await _db.ClassSchedules.AsNoTracking()
            .Where(x => x.TeacherId == teacherId && x.DayOfWeek == today && !x.IsDeleted)
            .OrderBy(x => x.TeachingPeriod!.SortOrder)
            .Select(x => new SimpleItemDto
            {
                Title = x.Subject!.NameAr,
                Subtitle = x.ClassSection!.NameAr + " - " + x.TeachingPeriod!.NameAr
            })
            .ToListAsync(cancellationToken);

        return new TeacherDashboardDto
        {
            TeacherNameAr = teacher.FullNameAr,
            TodayClasses = schedule.Count,
            AssignedSubjects = await _db.TeacherAssignments.CountAsync(x => x.TeacherId == teacherId && x.IsActive && !x.IsDeleted, cancellationToken),
            PendingHomeworkReviews = await _db.AssignmentSubmissions.CountAsync(x =>
                !x.IsDeleted &&
                x.Status == AssignmentStatus.Submitted &&
                x.Assignment!.TeacherId == teacherId, cancellationToken),
            UnreadMessages = await _db.MessageRecipients.CountAsync(x =>
                x.RecipientUserId == _currentUser.UserId && x.ReadAt == null, cancellationToken),
            TodaySchedule = schedule
        };
    }

    public async Task<ParentDashboardDto?> GetParentDashboardAsync(int guardianId, int? selectedStudentId = null, CancellationToken cancellationToken = default)
    {
        var guardian = await _db.Guardians.AsNoTracking().FirstOrDefaultAsync(x => x.Id == guardianId && !x.IsDeleted, cancellationToken);
        if (guardian is null || (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(guardian.SchoolId)))
        {
            return null;
        }

        var children = await (
            from link in _db.StudentGuardians.AsNoTracking()
            join student in _db.Students.AsNoTracking() on link.StudentId equals student.Id
            where link.GuardianId == guardianId && !student.IsDeleted
            select new ChildSummaryDto
            {
                StudentId = student.Id,
                FullNameAr = student.FullNameAr,
                StudentNumber = student.StudentNumber,
                GradeNameAr = _db.StudentEnrollments
                    .Where(e => e.StudentId == student.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => e.GradeLevel!.NameAr)
                    .FirstOrDefault(),
                PendingHomework = _db.Assignments.Count(a =>
                    !a.IsDeleted && a.Status == AssignmentStatus.Published && a.SchoolId == student.SchoolId &&
                    !_db.AssignmentSubmissions.Any(s => s.AssignmentId == a.Id && s.StudentId == student.Id && s.SubmittedAt != null))
            }).ToListAsync(cancellationToken);

        var selected = children.FirstOrDefault(x => x.StudentId == selectedStudentId) ?? children.FirstOrDefault();
        var feeBalance = selected is null
            ? 0m
            : await _db.StudentFees.Where(x => x.StudentId == selected.StudentId && !x.IsDeleted)
                .SumAsync(x => (decimal?)x.RemainingAmount, cancellationToken) ?? 0m;

        var announcements = await _db.Announcements.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsPublished && (x.SchoolId == null || x.SchoolId == guardian.SchoolId))
            .OrderByDescending(x => x.PublishAt)
            .Take(5)
            .Select(x => new AnnouncementCardDto
            {
                Id = x.Id,
                TitleAr = x.TitleAr,
                Priority = x.Priority.ToString(),
                PublishAt = x.PublishAt
            })
            .ToListAsync(cancellationToken);

        return new ParentDashboardDto
        {
            Children = children,
            SelectedChild = selected,
            FeeBalance = feeBalance,
            Announcements = announcements
        };
    }

    private List<int>? ResolveSchoolFilter(int? schoolId)
    {
        if (_currentUser.IsSuperAdmin)
        {
            return schoolId.HasValue ? [schoolId.Value] : null;
        }

        var ids = _currentUser.SchoolIds.ToList();
        if (schoolId.HasValue)
        {
            return ids.Contains(schoolId.Value) ? [schoolId.Value] : [];
        }

        return ids;
    }
}
