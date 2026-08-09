using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Exams;
using SchoolLMS.Domain.Entities.Operations;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Application.Services.Exams;

public class ExamAdminService : IExamAdminService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IAuditService _audit;
    private readonly IValidator<CreateExamRequest> _validator;

    public ExamAdminService(
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        IAuditService audit,
        IValidator<CreateExamRequest> validator)
    {
        _db = db;
        _currentUser = currentUser;
        _audit = audit;
        _validator = validator;
    }

    public async Task<IReadOnlyList<ExamListItemDto>> ListRecentAsync(int? schoolId, CancellationToken cancellationToken = default)
    {
        var query =
            from exam in _db.Exams.AsNoTracking()
            where !exam.IsDeleted
            join school in _db.Schools.AsNoTracking() on exam.SchoolId equals school.Id
            join subject in _db.Subjects.AsNoTracking() on exam.SubjectId equals subject.Id into sj
            from subject in sj.DefaultIfEmpty()
            join teacher in _db.Teachers.AsNoTracking() on exam.TeacherId equals teacher.Id into tg
            from teacher in tg.DefaultIfEmpty()
            join section in _db.ClassSections.AsNoTracking() on exam.ClassSectionId equals section.Id into sg
            from section in sg.DefaultIfEmpty()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id into gg
            from grade in gg.DefaultIfEmpty()
            join stage in _db.AcademicStages.AsNoTracking() on exam.AcademicStageId equals stage.Id into stg
            from stage in stg.DefaultIfEmpty()
            select new { exam, school, subject, teacher, section, grade, stage };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.exam.SchoolId));
        }

        if (schoolId.HasValue)
        {
            query = query.Where(x => x.exam.SchoolId == schoolId.Value);
        }

        var rows = await query
            .OrderByDescending(x => x.exam.ExamDate)
            .ThenByDescending(x => x.exam.Id)
            .Take(100)
            .Select(x => new
            {
                x.exam.Id,
                SchoolNameAr = x.school.NameAr,
                LessonNameAr = x.subject != null ? x.subject.NameAr : "—",
                TeacherNameAr = x.teacher != null ? x.teacher.FullNameAr : "—",
                StageNameAr = x.stage != null ? x.stage.NameAr : "—",
                GradeName = x.grade != null ? x.grade.NameAr : null,
                SectionName = x.section != null ? x.section.NameAr : null,
                x.exam.StartTime,
                x.exam.EndTime,
                x.exam.ExamDate,
                x.exam.Notes,
                x.exam.Status
            })
            .ToListAsync(cancellationToken);

        return rows.Select(x => new ExamListItemDto
        {
            Id = x.Id,
            SchoolNameAr = x.SchoolNameAr,
            LessonNameAr = x.LessonNameAr,
            TeacherNameAr = x.TeacherNameAr,
            StageNameAr = x.StageNameAr,
            ClassNameAr = x.GradeName != null && x.SectionName != null
                ? x.GradeName + " / " + x.SectionName
                : (x.SectionName ?? "—"),
            TimeSlot = FormatTimeSlot(x.StartTime, x.EndTime),
            ExamDate = x.ExamDate,
            StartTime = x.StartTime,
            Notes = x.Notes,
            Status = x.Status
        }).ToList();
    }

    public async Task<IReadOnlyList<StudentExamItemDto>> GetForCurrentStudentAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            return Array.Empty<StudentExamItemDto>();
        }

        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId && !x.IsDeleted, cancellationToken);
        if (student is null)
        {
            return Array.Empty<StudentExamItemDto>();
        }

        var enrollment = await (
            from e in _db.StudentEnrollments.AsNoTracking()
            join g in _db.GradeLevels.AsNoTracking() on e.GradeLevelId equals g.Id
            where e.StudentId == student.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted
            select new { e.ClassSectionId, StageId = g.AcademicStageId }
        ).FirstOrDefaultAsync(cancellationToken);

        if (enrollment is null)
        {
            return Array.Empty<StudentExamItemDto>();
        }

        var rows = await (
            from exam in _db.Exams.AsNoTracking()
            where !exam.IsDeleted
                  && exam.IsPublished
                  && exam.SchoolId == student.SchoolId
                  && exam.AcademicStageId == enrollment.StageId
                  && exam.ClassSectionId == enrollment.ClassSectionId
            join school in _db.Schools.AsNoTracking() on exam.SchoolId equals school.Id
            join subject in _db.Subjects.AsNoTracking() on exam.SubjectId equals subject.Id into sj
            from subject in sj.DefaultIfEmpty()
            join teacher in _db.Teachers.AsNoTracking() on exam.TeacherId equals teacher.Id into tg
            from teacher in tg.DefaultIfEmpty()
            join section in _db.ClassSections.AsNoTracking() on exam.ClassSectionId equals section.Id into sg
            from section in sg.DefaultIfEmpty()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id into gg
            from grade in gg.DefaultIfEmpty()
            join stage in _db.AcademicStages.AsNoTracking() on exam.AcademicStageId equals stage.Id into stg
            from stage in stg.DefaultIfEmpty()
            orderby exam.ExamDate, exam.StartTime
            select new
            {
                exam.Id,
                SchoolNameAr = school.NameAr,
                LessonNameAr = subject != null ? subject.NameAr : "—",
                TeacherNameAr = teacher != null ? teacher.FullNameAr : "—",
                StageNameAr = stage != null ? stage.NameAr : "—",
                GradeName = grade != null ? grade.NameAr : null,
                SectionName = section != null ? section.NameAr : null,
                exam.StartTime,
                exam.EndTime,
                exam.ExamDate,
                exam.Notes,
                exam.Instructions
            }
        ).ToListAsync(cancellationToken);

        return rows.Select(x => new StudentExamItemDto
        {
            Id = x.Id,
            SchoolNameAr = x.SchoolNameAr,
            LessonNameAr = x.LessonNameAr,
            TeacherNameAr = x.TeacherNameAr,
            StageNameAr = x.StageNameAr,
            ClassNameAr = x.GradeName != null && x.SectionName != null
                ? x.GradeName + " / " + x.SectionName
                : (x.SectionName ?? "—"),
            TimeSlot = FormatTimeSlot(x.StartTime, x.EndTime),
            ExamDate = x.ExamDate,
            Notes = x.Notes,
            Instructions = x.Instructions
        }).ToList();
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateExamRequest request, CancellationToken cancellationToken = default)
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

        var subjectOk = await _db.Subjects.AsNoTracking().AnyAsync(
            x => x.Id == request.SubjectId && x.SchoolId == request.SchoolId && !x.IsDeleted && x.IsActive,
            cancellationToken);
        if (!subjectOk)
        {
            return ServiceResult<int>.Failure("اسم الدرس غير مرتبط بالمدرسة المختارة.");
        }

        var sectionInfo = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.Id == request.ClassSectionId
                  && !section.IsDeleted
                  && section.IsActive
                  && section.SchoolId == request.SchoolId
                  && !grade.IsDeleted
                  && grade.IsActive
            select new
            {
                SectionId = section.Id,
                GradeLevelId = grade.Id,
                AcademicStageId = grade.AcademicStageId
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (sectionInfo is null)
        {
            return ServiceResult<int>.Failure("الشعبة غير صالحة لهذه المدرسة.");
        }

        var examDate = DateOnly.FromDateTime(request.ExamDateTime);
        var startTime = TimeOnly.FromDateTime(request.ExamDateTime);
        var endTime = startTime.AddHours(1);

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
            return ServiceResult<int>.Failure("لا توجد سنة دراسية للمدرسة.");
        }

        var examPeriod = await _db.ExamPeriods
            .FirstOrDefaultAsync(x => x.SchoolId == request.SchoolId
                                      && x.AcademicYearId == yearId.Value
                                      && x.NameAr == "امتحانات عامة"
                                      && !x.IsDeleted, cancellationToken);

        if (examPeriod is null)
        {
            examPeriod = new ExamPeriod
            {
                SchoolId = request.SchoolId,
                AcademicYearId = yearId.Value,
                NameAr = "امتحانات عامة",
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
                IsPublished = true
            };
            _db.ExamPeriods.Add(examPeriod);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var exam = new Exam
        {
            SchoolId = request.SchoolId,
            ExamPeriodId = examPeriod.Id,
            SubjectId = request.SubjectId,
            TeacherId = request.TeacherId,
            AcademicStageId = sectionInfo.AcademicStageId,
            GradeLevelId = sectionInfo.GradeLevelId,
            ClassSectionId = request.ClassSectionId,
            ExamType = ExamType.DailyQuiz,
            ExamDate = examDate,
            StartTime = startTime,
            EndTime = endTime,
            Notes = request.Notes?.Trim(),
            Instructions = request.Instructions?.Trim(),
            Status = request.Status,
            IsPublished = request.Status == PublicationStatus.Published,
            MaxScore = 100,
            PassScore = 50
        };

        _db.Exams.Add(exam);
        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("Exams.Create", nameof(Exam), exam.Id.ToString(),
            schoolId: exam.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(exam.Id);
    }

    private static string FormatTimeSlot(TimeOnly? start, TimeOnly? end)
    {
        if (!start.HasValue)
        {
            return "—";
        }

        return end.HasValue
            ? $"{start.Value:HH\\:mm} - {end.Value:HH\\:mm}"
            : $"{start.Value:HH\\:mm}";
    }
}
