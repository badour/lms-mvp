using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Application.Validators;
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
        db.Schools.Add(new School { NameAr = "مدرسة اختبار", NameEn = "Test School", IsActive = true });
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
            SchoolId = 1,
            StudentNumber = "S-100",
            FullNameAr = "طالب تجريبي",
            Gender = Gender.Male
        };

        var first = await service.CreateAsync(request);
        var second = await service.CreateAsync(request);

        first.Succeeded.Should().BeTrue();
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
