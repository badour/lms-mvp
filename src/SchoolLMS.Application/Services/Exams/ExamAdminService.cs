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
            join teacher in _db.Teachers.AsNoTracking() on exam.TeacherId equals teacher.Id into tg
            from teacher in tg.DefaultIfEmpty()
            join section in _db.ClassSections.AsNoTracking() on exam.ClassSectionId equals section.Id into sg
            from section in sg.DefaultIfEmpty()
            join stage in _db.AcademicStages.AsNoTracking() on exam.AcademicStageId equals stage.Id into stg
            from stage in stg.DefaultIfEmpty()
            select new { exam, teacher, section, stage };

        if (!_currentUser.IsSuperAdmin)
        {
            var schoolIds = _currentUser.SchoolIds;
            query = query.Where(x => schoolIds.Contains(x.exam.SchoolId));
        }

        if (schoolId.HasValue)
        {
            query = query.Where(x => x.exam.SchoolId == schoolId.Value);
        }

        return await query
            .OrderByDescending(x => x.exam.ExamDate)
            .ThenByDescending(x => x.exam.Id)
            .Take(50)
            .Select(x => new ExamListItemDto
            {
                Id = x.exam.Id,
                TeacherNameAr = x.teacher != null ? x.teacher.FullNameAr : "—",
                StageNameAr = x.stage != null ? x.stage.NameAr : "—",
                SectionNameAr = x.section != null ? x.section.NameAr : "—",
                ExamDate = x.exam.ExamDate,
                StartTime = x.exam.StartTime,
                Notes = x.exam.Notes,
                Status = x.exam.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateExamRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult<int>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var teacher = await _db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.TeacherId && !x.IsDeleted, cancellationToken);
        if (teacher is null)
        {
            return ServiceResult<int>.Failure("المعلم غير موجود.");
        }

        if (!_currentUser.IsSuperAdmin && !_currentUser.CanAccessSchool(teacher.SchoolId))
        {
            return ServiceResult<int>.Failure("غير مصرح بهذه المدرسة.");
        }

        var sectionInfo = await (
            from section in _db.ClassSections.AsNoTracking()
            join grade in _db.GradeLevels.AsNoTracking() on section.GradeLevelId equals grade.Id
            where section.Id == request.ClassSectionId
                  && !section.IsDeleted
                  && section.SchoolId == teacher.SchoolId
                  && grade.AcademicStageId == request.AcademicStageId
            select new { SectionId = section.Id, GradeLevelId = grade.Id }
        ).FirstOrDefaultAsync(cancellationToken);

        if (sectionInfo is null)
        {
            return ServiceResult<int>.Failure("الشعبة أو المرحلة غير صالحة.");
        }

        var subjectId = await _db.TeacherAssignments.AsNoTracking()
            .Where(x => x.TeacherId == teacher.Id
                        && x.ClassSectionId == request.ClassSectionId
                        && x.IsActive
                        && !x.IsDeleted)
            .Select(x => (int?)x.SubjectId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? await _db.Subjects.AsNoTracking()
                .Where(x => x.SchoolId == teacher.SchoolId && !x.IsDeleted)
                .OrderBy(x => x.NameAr)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        if (!subjectId.HasValue)
        {
            return ServiceResult<int>.Failure("لا توجد مادة مرتبطة لإنشاء الامتحان.");
        }

        var yearId = await _db.AcademicYears.AsNoTracking()
            .Where(x => x.SchoolId == teacher.SchoolId && x.IsCurrent && !x.IsDeleted)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? await _db.AcademicYears.AsNoTracking()
                .Where(x => x.SchoolId == teacher.SchoolId && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        if (!yearId.HasValue)
        {
            return ServiceResult<int>.Failure("لا توجد سنة دراسية للمدرسة.");
        }

        var period = await _db.ExamPeriods
            .FirstOrDefaultAsync(x => x.SchoolId == teacher.SchoolId
                                      && x.AcademicYearId == yearId.Value
                                      && x.NameAr == "امتحانات عامة"
                                      && !x.IsDeleted, cancellationToken);

        if (period is null)
        {
            period = new ExamPeriod
            {
                SchoolId = teacher.SchoolId,
                AcademicYearId = yearId.Value,
                NameAr = "امتحانات عامة",
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
                IsPublished = true
            };
            _db.ExamPeriods.Add(period);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var examDate = DateOnly.FromDateTime(request.ExamDateTime);
        var startTime = TimeOnly.FromDateTime(request.ExamDateTime);

        var exam = new Exam
        {
            SchoolId = teacher.SchoolId,
            ExamPeriodId = period.Id,
            SubjectId = subjectId.Value,
            TeacherId = teacher.Id,
            AcademicStageId = request.AcademicStageId,
            GradeLevelId = sectionInfo.GradeLevelId,
            ClassSectionId = request.ClassSectionId,
            ExamType = ExamType.DailyQuiz,
            ExamDate = examDate,
            StartTime = startTime,
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
}
