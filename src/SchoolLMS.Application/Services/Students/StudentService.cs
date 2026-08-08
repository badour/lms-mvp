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
                (x.s.FatherName != null && x.s.FatherName.Contains(term)) ||
                (x.s.Phone1 != null && x.s.Phone1.Contains(term)) ||
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
                FatherName = x.s.FatherName,
                Phone1 = x.s.Phone1,
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
                FatherName = x.FatherName,
                MotherName = x.MotherName,
                ProfileImagePath = x.ProfileImagePath,
                Gender = x.Gender,
                DateOfBirth = x.DateOfBirth,
                PlaceOfBirth = x.PlaceOfBirth,
                Nationality = x.Nationality,
                NationalId = x.NationalId,
                PassportOrCardId = x.PassportOrCardId ?? x.NationalId,
                RegistrationDate = x.RegistrationDate,
                AdmissionDate = x.AdmissionDate,
                ClassClassification = x.ClassClassification,
                Phone1 = x.Phone1,
                Phone2 = x.Phone2,
                City = x.City,
                Region = x.Region,
                Address = x.Address,
                Status = x.Status,
                PreviousSchool = x.PreviousSchool,
                Notes = x.Notes,
                SchoolNameAr = _db.Schools.Where(s => s.Id == x.SchoolId).Select(s => s.NameAr).FirstOrDefault() ?? string.Empty,
                BloodType = _db.StudentHealthProfiles.Where(h => h.StudentId == x.Id && !h.IsDeleted).Select(h => h.BloodType).FirstOrDefault(),
                DiseaseHistory = _db.StudentHealthProfiles.Where(h => h.StudentId == x.Id && !h.IsDeleted).Select(h => h.ChronicDiseases).FirstOrDefault(),
                EmergencyContactName = _db.EmergencyContacts.Where(c => c.StudentId == x.Id && !c.IsDeleted).OrderBy(c => c.PriorityOrder).Select(c => c.FullName).FirstOrDefault(),
                EmergencyContactPhone = _db.EmergencyContacts.Where(c => c.StudentId == x.Id && !c.IsDeleted).OrderBy(c => c.PriorityOrder).Select(c => c.Phone).FirstOrDefault(),
                AcademicYearId = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => (int?)e.AcademicYearId).FirstOrDefault(),
                GradeLevelId = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => (int?)e.GradeLevelId).FirstOrDefault(),
                ClassSectionId = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => (int?)e.ClassSectionId).FirstOrDefault(),
                GradeNameAr = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => e.GradeLevel!.NameAr).FirstOrDefault(),
                SectionNameAr = _db.StudentEnrollments
                    .Where(e => e.StudentId == x.Id && e.Status == EnrollmentStatus.Active && !e.IsDeleted)
                    .Select(e => e.ClassSection!.NameAr).FirstOrDefault(),
                Hobbies = _db.StudentHobbies.Where(h => h.StudentId == x.Id && !h.IsDeleted).OrderBy(h => h.SortOrder).Select(h => h.Name).ToList(),
                NotesList = _db.StudentNotes.Where(n => n.StudentId == x.Id && !n.IsDeleted).OrderBy(n => n.SortOrder).Select(n => n.NoteText).ToList()
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

        var studentNumber = string.IsNullOrWhiteSpace(request.StudentNumber)
            ? await GenerateStudentNumberAsync(request.SchoolId, cancellationToken)
            : request.StudentNumber.Trim();

        var exists = await _db.Students.AnyAsync(x =>
            x.SchoolId == request.SchoolId &&
            x.StudentNumber == studentNumber &&
            !x.IsDeleted, cancellationToken);

        if (exists)
        {
            return ServiceResult<int>.Failure("رقم الطالب مستخدم مسبقاً في هذه المدرسة.");
        }

        var student = new Student
        {
            SchoolId = request.SchoolId,
            SchoolBranchId = request.SchoolBranchId,
            StudentNumber = studentNumber,
            FullNameAr = request.FullNameAr.Trim(),
            FullNameEn = request.FullNameEn?.Trim(),
            FatherName = request.FatherName.Trim(),
            MotherName = request.MotherName.Trim(),
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            PlaceOfBirth = request.PlaceOfBirth,
            Nationality = request.Nationality,
            NationalId = request.PassportOrCardId.Trim(),
            PassportOrCardId = request.PassportOrCardId.Trim(),
            AdmissionDate = request.AdmissionDate,
            RegistrationDate = request.AdmissionDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            ClassClassification = request.ClassClassification?.Trim(),
            Phone1 = request.Phone1.Trim(),
            Phone2 = request.Phone2?.Trim(),
            City = request.City.Trim(),
            Region = request.Region.Trim(),
            Address = request.Address.Trim(),
            PreviousSchool = request.PreviousSchool,
            Notes = request.Notes,
            Status = StudentStatus.Active,
            HealthProfile = new StudentHealthProfile
            {
                SchoolId = request.SchoolId,
                BloodType = request.BloodType.Trim().ToUpperInvariant(),
                ChronicDiseases = request.DiseaseHistory?.Trim()
            }
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync(cancellationToken);

        var father = new Guardian
        {
            SchoolId = request.SchoolId,
            FullNameAr = request.FatherName.Trim(),
            Phone = request.Phone1.Trim()
        };
        _db.Guardians.Add(father);
        await _db.SaveChangesAsync(cancellationToken);

        _db.StudentGuardians.Add(new StudentGuardian
        {
            StudentId = student.Id,
            GuardianId = father.Id,
            Relationship = "أب",
            IsPrimary = true,
            IsFinanciallyResponsible = true,
            CanReceiveNotifications = true,
            CanCollectStudent = true
        });

        var mother = new Guardian
        {
            SchoolId = request.SchoolId,
            FullNameAr = request.MotherName.Trim(),
            Phone = request.Phone2 ?? request.Phone1
        };
        _db.Guardians.Add(mother);
        await _db.SaveChangesAsync(cancellationToken);

        _db.StudentGuardians.Add(new StudentGuardian
        {
            StudentId = student.Id,
            GuardianId = mother.Id,
            Relationship = "أم",
            IsPrimary = false,
            IsFinanciallyResponsible = false,
            CanReceiveNotifications = true,
            CanCollectStudent = true
        });

        _db.StudentAddresses.Add(new StudentAddress
        {
            SchoolId = request.SchoolId,
            StudentId = student.Id,
            Country = "العراق",
            Governorate = request.City,
            District = request.Region,
            DetailedAddress = request.Address
        });

        if (!string.IsNullOrWhiteSpace(request.EmergencyContactName) || !string.IsNullOrWhiteSpace(request.EmergencyContactPhone))
        {
            _db.EmergencyContacts.Add(new EmergencyContact
            {
                SchoolId = request.SchoolId,
                StudentId = student.Id,
                FullName = request.EmergencyContactName?.Trim() ?? "جهة طوارئ",
                Relationship = "طوارئ",
                Phone = request.EmergencyContactPhone?.Trim() ?? string.Empty,
                PriorityOrder = 1
            });
        }

        var hobbyOrder = 1;
        foreach (var hobby in request.Hobbies.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct())
        {
            _db.StudentHobbies.Add(new StudentHobby
            {
                SchoolId = request.SchoolId,
                StudentId = student.Id,
                Name = hobby,
                SortOrder = hobbyOrder++
            });
        }

        var noteOrder = 1;
        foreach (var note in request.NotesList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
        {
            _db.StudentNotes.Add(new StudentNote
            {
                SchoolId = request.SchoolId,
                StudentId = student.Id,
                NoteText = note,
                NoteDate = DateTime.UtcNow,
                SortOrder = noteOrder++
            });
        }

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
                EnrollmentDate = request.AdmissionDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
            });
        }
        else if (request.GradeLevelId.HasValue && request.ClassSectionId.HasValue)
        {
            var currentYearId = await _db.AcademicYears.AsNoTracking()
                .Where(x => x.SchoolId == request.SchoolId && x.IsCurrent && !x.IsDeleted)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentYearId.HasValue)
            {
                _db.StudentEnrollments.Add(new StudentEnrollment
                {
                    SchoolId = request.SchoolId,
                    StudentId = student.Id,
                    AcademicYearId = currentYearId.Value,
                    GradeLevelId = request.GradeLevelId.Value,
                    ClassSectionId = request.ClassSectionId.Value,
                    Status = EnrollmentStatus.Active,
                    EnrollmentDate = request.AdmissionDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("Student.Create", nameof(Student), student.Id.ToString(),
            newValues: request, schoolId: student.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult<int>.Success(student.Id);
    }

    private async Task<string> GenerateStudentNumberAsync(int schoolId, CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _db.Students.CountAsync(x => x.SchoolId == schoolId && !x.IsDeleted, cancellationToken);
        return $"S{schoolId:000}-{year}-{(count + 1):0000}";
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

        var newNumber = string.IsNullOrWhiteSpace(request.StudentNumber)
            ? student.StudentNumber
            : request.StudentNumber.Trim();

        var duplicate = await _db.Students.AnyAsync(x =>
            x.Id != request.Id &&
            x.SchoolId == student.SchoolId &&
            x.StudentNumber == newNumber &&
            !x.IsDeleted, cancellationToken);

        if (duplicate)
        {
            return ServiceResult.Failure("رقم الطالب مستخدم مسبقاً في هذه المدرسة.");
        }

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ServiceResult.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        if (request.SchoolId != student.SchoolId)
        {
            if (!_currentUser.CanAccessSchool(request.SchoolId) && !_currentUser.IsSuperAdmin)
            {
                return ServiceResult.Failure("غير مصرح بنقل الطالب إلى هذه المدرسة.");
            }

            student.SchoolId = request.SchoolId;
        }

        student.StudentNumber = newNumber;
        student.FullNameAr = request.FullNameAr.Trim();
        student.FullNameEn = request.FullNameEn?.Trim();
        student.FatherName = request.FatherName.Trim();
        student.MotherName = request.MotherName.Trim();
        student.Gender = request.Gender;
        student.DateOfBirth = request.DateOfBirth;
        student.PlaceOfBirth = request.PlaceOfBirth;
        student.Nationality = request.Nationality;
        student.NationalId = request.PassportOrCardId?.Trim() ?? request.NationalId;
        student.PassportOrCardId = request.PassportOrCardId?.Trim();
        student.AdmissionDate = request.AdmissionDate;
        student.ClassClassification = request.ClassClassification?.Trim();
        student.Phone1 = request.Phone1.Trim();
        student.Phone2 = request.Phone2?.Trim();
        student.City = request.City.Trim();
        student.Region = request.Region.Trim();
        student.Address = request.Address.Trim();
        student.PreviousSchool = request.PreviousSchool;
        student.Notes = request.Notes;
        student.Status = request.Status;
        student.SchoolBranchId = request.SchoolBranchId;

        var health = await _db.StudentHealthProfiles
            .FirstOrDefaultAsync(x => x.StudentId == student.Id && !x.IsDeleted, cancellationToken);
        if (health is null)
        {
            health = new StudentHealthProfile { SchoolId = student.SchoolId, StudentId = student.Id };
            _db.StudentHealthProfiles.Add(health);
        }

        health.SchoolId = student.SchoolId;
        health.BloodType = request.BloodType.Trim().ToUpperInvariant();
        health.ChronicDiseases = request.DiseaseHistory?.Trim();

        if (request.GradeLevelId.HasValue && request.ClassSectionId.HasValue)
        {
            var yearId = request.AcademicYearId
                ?? await _db.AcademicYears.AsNoTracking()
                    .Where(x => x.SchoolId == student.SchoolId && x.IsCurrent && !x.IsDeleted)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (yearId.HasValue)
            {
                var enrollment = await _db.StudentEnrollments
                    .Where(x => x.StudentId == student.Id && !x.IsDeleted && x.Status == EnrollmentStatus.Active)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (enrollment is null)
                {
                    _db.StudentEnrollments.Add(new StudentEnrollment
                    {
                        SchoolId = student.SchoolId,
                        StudentId = student.Id,
                        AcademicYearId = yearId.Value,
                        GradeLevelId = request.GradeLevelId.Value,
                        ClassSectionId = request.ClassSectionId.Value,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = request.AdmissionDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
                    });
                }
                else
                {
                    enrollment.SchoolId = student.SchoolId;
                    enrollment.AcademicYearId = yearId.Value;
                    enrollment.GradeLevelId = request.GradeLevelId.Value;
                    enrollment.ClassSectionId = request.ClassSectionId.Value;
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Student.Update", nameof(Student), student.Id.ToString(),
            newValues: request, schoolId: student.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (student is null)
        {
            return ServiceResult.Failure("الطالب غير موجود.");
        }

        if (!_currentUser.CanAccessSchool(student.SchoolId) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("غير مصرح بحذف هذا الطالب.");
        }

        if (!_currentUser.HasPermission(PermissionNames.StudentsDelete) && !_currentUser.IsSuperAdmin)
        {
            return ServiceResult.Failure("ليس لديك صلاحية حذف الطالب.");
        }

        var now = DateTime.UtcNow;
        student.IsDeleted = true;
        student.DeletedAt = now;
        student.DeletedByUserId = _currentUser.UserId;
        student.Status = StudentStatus.Withdrawn;

        var enrollments = await _db.StudentEnrollments
            .Where(x => x.StudentId == student.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var enrollment in enrollments)
        {
            enrollment.IsDeleted = true;
            enrollment.DeletedAt = now;
            enrollment.Status = EnrollmentStatus.Withdrawn;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Student.Delete", nameof(Student), student.Id.ToString(),
            schoolId: student.SchoolId, cancellationToken: cancellationToken);

        return ServiceResult.Success();
    }
}
