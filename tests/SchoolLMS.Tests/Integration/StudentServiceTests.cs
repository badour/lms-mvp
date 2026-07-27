using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Application.Validators;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Entities.Tenancy;
using SchoolLMS.Domain.Enums;
using SchoolLMS.Domain.Interfaces;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Tests.Integration;

public class StudentServiceTests
{
    [Fact]
    public async Task CreateAsync_rejects_duplicate_student_number_in_same_school()
    {
        await using var db = CreateDb();
        var school = new School { NameAr = "مدرسة اختبار", NameEn = "Test School", IsActive = true };
        db.Schools.Add(school);
        await db.SaveChangesAsync();

        var stage = new AcademicStage { SchoolId = school.Id, NameAr = "ابتدائي", NameEn = "Primary", SortOrder = 1 };
        db.AcademicStages.Add(stage);
        await db.SaveChangesAsync();

        var grade = new GradeLevel { SchoolId = school.Id, AcademicStageId = stage.Id, NameAr = "الخامس", NameEn = "G5", SortOrder = 5 };
        db.GradeLevels.Add(grade);
        await db.SaveChangesAsync();

        var section = new ClassSection { SchoolId = school.Id, GradeLevelId = grade.Id, NameAr = "أ", NameEn = "A" };
        db.ClassSections.Add(section);
        await db.SaveChangesAsync();

        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.IsSuperAdmin).Returns(true);
        currentUser.Setup(x => x.CanAccessSchool(It.IsAny<int>())).Returns(true);
        currentUser.Setup(x => x.HasPermission(It.IsAny<string>())).Returns(true);

        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.LogAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<object?>(), It.IsAny<object?>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new StudentService(db, currentUser.Object, audit.Object, new CreateStudentRequestValidator());

        var request = new CreateStudentRequest
        {
            SchoolId = school.Id,
            StudentNumber = "S-100",
            FullNameAr = "طالب تجريبي",
            FatherName = "والد تجريبي",
            MotherName = "والدة تجريبية",
            PassportOrCardId = "A1234567",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(2014, 1, 1),
            AdmissionDate = DateOnly.FromDateTime(DateTime.Today),
            GradeLevelId = grade.Id,
            ClassSectionId = section.Id,
            Phone1 = "07700000000",
            City = "بغداد",
            Region = "الكرادة",
            Address = "شارع تجريبي",
            BloodType = "O+",
            Hobbies = ["قراءة"],
            NotesList = ["ملاحظة أولى"]
        };

        var first = await service.CreateAsync(request);
        var second = await service.CreateAsync(request);

        first.Succeeded.Should().BeTrue(first.Error);
        second.Succeeded.Should().BeFalse();
        second.Error.Should().Contain("رقم الطالب");
    }

    private static ApplicationDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source=file:student-tests-{Guid.NewGuid():N}?mode=memory&cache=shared")
            .Options;

        var db = new ApplicationDbContext(options);
        db.Database.OpenConnection();
        db.Database.EnsureCreated();
        return db;
    }
}
