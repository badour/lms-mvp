# Phase 1A — Project Folder Structure

Proposed solution layout (created in Phase 1B; documented now for approval).

```text
SchoolLMS.sln
README.md
.gitignore
.editorconfig
Directory.Build.props                 # shared build settings (nullable, warnings)
docker-compose.yml                    # optional local SQL Server later

docs/
  phase-1a/                           # this design package
  deployment/
    iis-deployment.md                 # Phase 8
    sql-server-backup.md              # Phase 8
    production-security-checklist.md  # Phase 8
  erd/                                # exported diagrams if needed

src/
  SchoolLMS.Domain/
    Common/
      AuditableEntity.cs
      SchoolOwnedEntity.cs
      ISoftDelete.cs
      Result.cs
    Enums/
      UserStatus.cs
      EnrollmentStatus.cs
      AttendanceStatus.cs
      AssignmentStatus.cs
      GradeWorkflowStatus.cs
      AnnouncementPriority.cs
      PaymentStatus.cs
      ...
    Entities/
      Tenancy/
        School.cs
        SchoolBranch.cs
        SchoolSettings.cs
      Identity/
        ApplicationUser.cs
        Permission.cs
        RolePermission.cs
        UserPermission.cs
        UserSchoolAssignment.cs
        UserSchoolRole.cs
        LoginHistory.cs
        AuditLog.cs
      Academic/
        AcademicYear.cs
        Semester.cs
        AcademicStage.cs
        GradeLevel.cs
        ClassSection.cs
        Subject.cs
        ...
      People/
        Student.cs
        Guardian.cs
        StudentGuardian.cs
        Teacher.cs
        Employee.cs
        ...
      Lms/
        CourseUnit.cs
        Lesson.cs
        Assignment.cs
        Quiz.cs
        ...
      Operations/
        Exam.cs
        StudentGrade.cs
        AttendanceSession.cs
        ...
      Communication/
        Announcement.cs
        Notification.cs
        Message.cs
        ...
      Finance/
        FeeType.cs
        Payment.cs
        ...
      Transport/
      Library/
    Interfaces/
      IRepository.cs
      IUnitOfWork.cs
      ICurrentUserContext.cs
      IFileStorage.cs
      IAuditService.cs
    ValueObjects/
      Money.cs
      PhoneNumber.cs
      Address.cs

  SchoolLMS.Application/
    DependencyInjection.cs
    Common/
      Interfaces/
      Models/
      Exceptions/
      Behaviours/                     # optional pipeline behaviours
    DTOs/
    ViewModels/
    Mappings/
    Validators/
    Services/
      Schools/
      Identity/
      Students/
      Academic/
      Lms/
      Attendance/
      Grades/
      Notifications/
      Fees/
      Reports/
    Authorization/
      PermissionNames.cs
      PermissionRequirement.cs
    UseCases/                         # optional vertical slices for complex flows

  SchoolLMS.Infrastructure/
    DependencyInjection.cs
    Persistence/
      ApplicationDbContext.cs
      Configurations/                 # IEntityTypeConfiguration<>
      Interceptors/                   # audit + soft delete
      Migrations/
      Seed/
        RolePermissionSeeder.cs
        DemoDataSeeder.cs
    Identity/
      IdentityService.cs
      JwtTokenService.cs
    Repositories/
    Services/
      FileStorage/
        LocalFileStorage.cs
        AzureBlobFileStorage.cs       # stub/provider-ready
      Notifications/
        EmailSender.cs
        SmsSender.cs
        PushNotificationSender.cs
      Audit/
        AuditService.cs
    BackgroundJobs/
      HangfireExtensions.cs
      Jobs/
    Logging/
      SerilogConfiguration.cs

  SchoolLMS.Web/
    Program.cs
    appsettings.json
    appsettings.Development.json
    Areas/
      Admin/
        Controllers/
        Views/
      Teacher/
      Student/
      Parent/
    Controllers/                      # account, public
    Views/
      Shared/
        _Layout.cshtml                # RTL shell
        _Sidebar.cshtml
        _TopNav.cshtml
        Components/
      Account/
    wwwroot/
      css/
      js/
      lib/
      images/
    Resources/
      SharedResource.ar.resx
      SharedResource.en.resx
    Middleware/
    Filters/

  SchoolLMS.Api/
    Program.cs
    Controllers/
      v1/
        AuthController.cs
        StudentsController.cs
        ParentsController.cs
        TeachersController.cs
        AnnouncementsController.cs
        NotificationsController.cs
    Contracts/
      Requests/
      Responses/
    Middleware/
    Filters/
    Swagger/

tests/
  SchoolLMS.Tests/
    Unit/
      Application/
      Domain/
    Integration/
      Persistence/
      Api/
    Authorization/
```

## Project references

```text
SchoolLMS.Web            → Application, Infrastructure
SchoolLMS.Api            → Application, Infrastructure
SchoolLMS.Infrastructure → Application, Domain
SchoolLMS.Application    → Domain
SchoolLMS.Tests          → Web, Api, Application, Infrastructure, Domain
```

Infrastructure references Application only for DI registration of service implementations against Application interfaces (classic clean architecture variant). Domain remains dependency-free.
