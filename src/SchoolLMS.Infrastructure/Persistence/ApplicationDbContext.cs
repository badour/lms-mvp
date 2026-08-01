using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Common;
using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Entities;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Entities.Communication;
using SchoolLMS.Domain.Entities.Finance;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Domain.Entities.Library;
using SchoolLMS.Domain.Entities.Lms;
using SchoolLMS.Domain.Entities.Operations;
using SchoolLMS.Domain.Entities.People;
using SchoolLMS.Domain.Entities.Tenancy;
using SchoolLMS.Domain.Entities.Transport;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly ICurrentUserContext? _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserContext? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<School> Schools => Set<School>();
    public DbSet<SchoolBranch> SchoolBranches => Set<SchoolBranch>();
    public DbSet<SchoolSettings> SchoolSettings => Set<SchoolSettings>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<UserSchoolAssignment> UserSchoolAssignments => Set<UserSchoolAssignment>();
    public DbSet<UserSchoolRole> UserSchoolRoles => Set<UserSchoolRole>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<AcademicStage> AcademicStages => Set<AcademicStage>();
    public DbSet<GradeLevel> GradeLevels => Set<GradeLevel>();
    public DbSet<ClassSection> ClassSections => Set<ClassSection>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SubjectGradeAssignment> SubjectGradeAssignments => Set<SubjectGradeAssignment>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<TeachingPeriod> TeachingPeriods => Set<TeachingPeriod>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();
    public DbSet<TeacherAssignment> TeacherAssignments => Set<TeacherAssignment>();

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    public DbSet<StudentAddress> StudentAddresses => Set<StudentAddress>();
    public DbSet<StudentHealthProfile> StudentHealthProfiles => Set<StudentHealthProfile>();
    public DbSet<StudentEducationalProfile> StudentEducationalProfiles => Set<StudentEducationalProfile>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<StudentHobby> StudentHobbies => Set<StudentHobby>();
    public DbSet<StudentNote> StudentNotes => Set<StudentNote>();
    public DbSet<StudentQimamCertificate> StudentQimamCertificates => Set<StudentQimamCertificate>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<CourseUnit> CourseUnits => Set<CourseUnit>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonResource> LessonResources => Set<LessonResource>();
    public DbSet<LessonClassSection> LessonClassSections => Set<LessonClassSection>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();

    public DbSet<ExamPeriod> ExamPeriods => Set<ExamPeriod>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<GradeCategory> GradeCategories => Set<GradeCategory>();
    public DbSet<GradeItem> GradeItems => Set<GradeItem>();
    public DbSet<StudentGrade> StudentGrades => Set<StudentGrade>();
    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();
    public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
    public DbSet<AttendanceExcuse> AttendanceExcuses => Set<AttendanceExcuse>();

    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementTarget> AnnouncementTargets => Set<AnnouncementTarget>();
    public DbSet<SchoolEvent> SchoolEvents => Set<SchoolEvent>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageRecipient> MessageRecipients => Set<MessageRecipient>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<BehaviourCategory> BehaviourCategories => Set<BehaviourCategory>();
    public DbSet<BehaviourRecord> BehaviourRecords => Set<BehaviourRecord>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<StudentBadge> StudentBadges => Set<StudentBadge>();

    public DbSet<FeeType> FeeTypes => Set<FeeType>();
    public DbSet<FeePlan> FeePlans => Set<FeePlan>();
    public DbSet<StudentFee> StudentFees => Set<StudentFee>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<TransportRoute> TransportRoutes => Set<TransportRoute>();
    public DbSet<StudentTransportAssignment> StudentTransportAssignments => Set<StudentTransportAssignment>();

    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<LibraryLoan> LibraryLoans => Set<LibraryLoan>();
    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<School>(e =>
        {
            e.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
            e.Property(x => x.NameEn).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.NameAr);
            e.HasOne(x => x.Settings).WithOne(x => x.School).HasForeignKey<SchoolSettings>(x => x.SchoolId);
        });

        builder.Entity<SchoolBranch>(e =>
        {
            e.HasIndex(x => new { x.SchoolId, x.NameAr });
            e.HasOne(x => x.School).WithMany(x => x.Branches).HasForeignKey(x => x.SchoolId);
        });

        builder.Entity<Permission>(e =>
        {
            e.HasIndex(x => x.Key).IsUnique();
            e.Property(x => x.Key).HasMaxLength(100).IsRequired();
        });

        builder.Entity<RolePermission>(e =>
        {
            e.HasKey(x => new { x.RoleId, x.PermissionId });
            e.HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);
        });

        builder.Entity<UserPermission>(e =>
        {
            e.HasKey(x => new { x.UserId, x.PermissionId });
            e.HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        builder.Entity<UserSchoolAssignment>(e =>
        {
            e.HasIndex(x => new { x.UserId, x.SchoolId });
            e.HasOne(x => x.User).WithMany(x => x.SchoolAssignments).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.School).WithMany().HasForeignKey(x => x.SchoolId);
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
        });

        builder.Entity<Student>(e =>
        {
            e.Property(x => x.StudentNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.FullNameAr).HasMaxLength(200).IsRequired();
            e.Property(x => x.FatherName).HasMaxLength(200);
            e.Property(x => x.MotherName).HasMaxLength(200);
            e.Property(x => x.PassportOrCardId).HasMaxLength(100);
            e.Property(x => x.ClassClassification).HasMaxLength(100);
            e.Property(x => x.Phone1).HasMaxLength(30);
            e.Property(x => x.Phone2).HasMaxLength(30);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.Region).HasMaxLength(100);
            e.Property(x => x.Address).HasMaxLength(500);
            e.HasIndex(x => new { x.SchoolId, x.StudentNumber }).IsUnique();
            e.HasIndex(x => x.SchoolId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.PassportOrCardId);
            e.HasMany(x => x.Hobbies).WithOne(x => x.Student).HasForeignKey(x => x.StudentId);
            e.HasMany(x => x.StudentNotes).WithOne(x => x.Student).HasForeignKey(x => x.StudentId);
            e.HasMany(x => x.EmergencyContacts).WithOne(x => x.Student).HasForeignKey(x => x.StudentId);
        });

        builder.Entity<StudentHobby>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.HasIndex(x => x.StudentId);
        });

        builder.Entity<StudentNote>(e =>
        {
            e.Property(x => x.NoteText).HasMaxLength(2000).IsRequired();
            e.HasIndex(x => x.StudentId);
        });

        builder.Entity<StudentQimamCertificate>(e =>
        {
            e.Property(x => x.CertificateName).HasMaxLength(250).IsRequired();
            e.Property(x => x.ClassName).HasMaxLength(150).IsRequired();
            e.Property(x => x.ImagePath).HasMaxLength(500);
            e.Property(x => x.ImageOriginalName).HasMaxLength(300);
            e.Property(x => x.DocumentPath).HasMaxLength(500);
            e.Property(x => x.DocumentOriginalName).HasMaxLength(300);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.Property(x => x.Description).HasMaxLength(4000);
            e.HasIndex(x => x.StudentId);
            e.HasIndex(x => new { x.SchoolId, x.CertificateDate });
            e.HasOne(x => x.Student).WithMany(x => x.QimamCertificates).HasForeignKey(x => x.StudentId);
        });

        builder.Entity<Message>(e =>
        {
            e.Property(x => x.SenderUserId).HasMaxLength(450).IsRequired();
            e.Property(x => x.SenderDisplayName).HasMaxLength(200);
            e.Property(x => x.Subject).HasMaxLength(250).IsRequired();
            e.Property(x => x.Body).HasMaxLength(4000).IsRequired();
            e.Property(x => x.Category).HasMaxLength(50).IsRequired();
            e.Property(x => x.TargetDisplayName).HasMaxLength(200);
            e.Property(x => x.ReplyBody).HasMaxLength(4000);
            e.Property(x => x.RepliedByUserId).HasMaxLength(450);
            e.HasIndex(x => new { x.SchoolId, x.StudentId, x.CreatedAt });
            e.HasIndex(x => x.SenderUserId);
            e.HasIndex(x => x.ParentMessageId);
            e.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId);
            e.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId);
            e.HasOne(x => x.ParentMessage).WithMany(x => x.Replies).HasForeignKey(x => x.ParentMessageId);
            e.HasMany(x => x.Recipients).WithOne(x => x.Message).HasForeignKey(x => x.MessageId);
            e.HasMany(x => x.Attachments).WithOne(x => x.Message).HasForeignKey(x => x.MessageId);
        });

        builder.Entity<MessageRecipient>(e =>
        {
            e.Property(x => x.RecipientUserId).HasMaxLength(450).IsRequired();
            e.HasIndex(x => x.MessageId);
            e.HasIndex(x => x.RecipientUserId);
        });

        builder.Entity<MessageAttachment>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(250).IsRequired();
            e.Property(x => x.OriginalFileName).HasMaxLength(300).IsRequired();
            e.Property(x => x.RelativePath).HasMaxLength(500).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(150);
            e.HasIndex(x => x.MessageId);
        });

        builder.Entity<StudentGuardian>(e =>
        {
            e.HasKey(x => new { x.StudentId, x.GuardianId });
            e.HasOne(x => x.Student).WithMany(x => x.Guardians).HasForeignKey(x => x.StudentId);
            e.HasOne(x => x.Guardian).WithMany(x => x.Students).HasForeignKey(x => x.GuardianId);
        });

        builder.Entity<StudentEnrollment>(e =>
        {
            e.HasIndex(x => new { x.StudentId, x.AcademicYearId });
            e.HasIndex(x => x.SchoolId);
            e.HasOne(x => x.AcademicYear).WithMany().HasForeignKey(x => x.AcademicYearId);
            e.HasOne(x => x.GradeLevel).WithMany().HasForeignKey(x => x.GradeLevelId);
            e.HasOne(x => x.ClassSection).WithMany().HasForeignKey(x => x.ClassSectionId);
        });

        builder.Entity<AcademicStage>(e =>
        {
            e.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
            e.Property(x => x.NameEn).HasMaxLength(200);
            e.Property(x => x.YearName).HasMaxLength(100);
            e.HasIndex(x => new { x.SchoolId, x.IsActive });
        });

        builder.Entity<ClassSchedule>(e =>
        {
            e.Property(x => x.EntryText).HasMaxLength(200);
            e.HasIndex(x => new { x.TeacherId, x.DayOfWeek, x.TeachingPeriodId });
            e.HasIndex(x => new { x.ClassSectionId, x.DayOfWeek, x.TeachingPeriodId });
            e.HasOne(x => x.ClassSection).WithMany().HasForeignKey(x => x.ClassSectionId);
            e.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId);
            e.HasOne(x => x.TeachingPeriod).WithMany().HasForeignKey(x => x.TeachingPeriodId);
            e.HasOne(x => x.Classroom).WithMany().HasForeignKey(x => x.ClassroomId);
        });

        builder.Entity<Teacher>(e =>
        {
            e.Property(x => x.FullNameAr).HasMaxLength(200).IsRequired();
            e.Property(x => x.FullNameEn).HasMaxLength(200);
            e.Property(x => x.DocumentId).HasMaxLength(100);
            e.Property(x => x.ParentName).HasMaxLength(200);
            e.Property(x => x.MotherName).HasMaxLength(200);
            e.Property(x => x.RoleName).HasMaxLength(100);
            e.Property(x => x.Specialization).HasMaxLength(200);
            e.Property(x => x.EducationalInfo).HasMaxLength(4000);
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.AttachmentPath).HasMaxLength(500);
            e.Property(x => x.AttachmentOriginalName).HasMaxLength(300);
            e.Property(x => x.AttachmentContentType).HasMaxLength(150);
            e.HasIndex(x => x.SchoolId);
            e.HasIndex(x => x.DocumentId);
            e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId);
        });

        builder.Entity<Exam>(e =>
        {
            e.Property(x => x.MaxScore).HasPrecision(18, 2);
            e.Property(x => x.PassScore).HasPrecision(18, 2);
            e.Property(x => x.Topics).HasMaxLength(2000);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.Property(x => x.Instructions).HasMaxLength(4000);
            e.HasIndex(x => new { x.SchoolId, x.ExamDate });
            e.HasIndex(x => x.TeacherId);
            e.HasIndex(x => x.ClassSectionId);
            e.HasOne(x => x.ExamPeriod).WithMany().HasForeignKey(x => x.ExamPeriodId);
        });

        builder.Entity<StudentAttendance>(e =>
        {
            e.HasIndex(x => new { x.AttendanceSessionId, x.StudentId }).IsUnique();
            e.HasOne(x => x.AttendanceSession).WithMany(x => x.Records).HasForeignKey(x => x.AttendanceSessionId);
        });

        builder.Entity<QuizQuestion>(e =>
        {
            e.HasKey(x => new { x.QuizId, x.QuestionId });
            e.HasOne(x => x.Quiz).WithMany(x => x.Questions).HasForeignKey(x => x.QuizId);
            e.HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId);
        });

        builder.Entity<Payment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(18, 2);
        });

        builder.Entity<StudentFee>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.PaidAmount).HasPrecision(18, 2);
            e.Property(x => x.RemainingAmount).HasPrecision(18, 2);
        });

        builder.Entity<FeePlan>(e => e.Property(x => x.Amount).HasPrecision(18, 2));
        builder.Entity<PaymentAllocation>(e => e.Property(x => x.Amount).HasPrecision(18, 2));
        builder.Entity<LibraryLoan>(e => e.Property(x => x.PenaltyAmount).HasPrecision(18, 2));
        builder.Entity<Assignment>(e => e.Property(x => x.TotalMarks).HasPrecision(18, 2));
        builder.Entity<StudentGrade>(e => e.Property(x => x.Score).HasPrecision(18, 2));
        builder.Entity<LessonProgress>(e => e.Property(x => x.VideoCompletionPercent).HasPrecision(5, 2));

        builder.Entity<Lesson>(e =>
        {
            e.Property(x => x.TitleAr).HasMaxLength(250).IsRequired();
            e.Property(x => x.TitleEn).HasMaxLength(250);
            e.Property(x => x.Description).HasMaxLength(4000);
            e.Property(x => x.LearningObjectives).HasMaxLength(2000);
            e.Property(x => x.VideoUrl).HasMaxLength(1000);
            e.Property(x => x.VideoPath).HasMaxLength(500);
            e.Property(x => x.VideoOriginalName).HasMaxLength(300);
            e.Property(x => x.TeacherNotes).HasMaxLength(2000);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.Property(x => x.StudentInstructions).HasMaxLength(2000);
            e.Property(x => x.FeaturedImagePath).HasMaxLength(500);
            e.HasIndex(x => new { x.SchoolId, x.SubjectId, x.Status });
            e.HasIndex(x => x.TeacherId);
            e.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId);
            e.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId);
            e.HasOne(x => x.CourseUnit).WithMany(x => x.Lessons).HasForeignKey(x => x.CourseUnitId);
            e.HasMany(x => x.Resources).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId);
            e.HasMany(x => x.IncludedClasses).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId);
        });

        builder.Entity<LessonClassSection>(e =>
        {
            e.HasKey(x => new { x.LessonId, x.ClassSectionId });
            e.HasOne(x => x.ClassSection).WithMany().HasForeignKey(x => x.ClassSectionId);
        });

        builder.Entity<LessonResource>(e =>
        {
            e.Property(x => x.ResourceType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Title).HasMaxLength(250).IsRequired();
            e.Property(x => x.Url).HasMaxLength(1000);
            e.Property(x => x.RelativePath).HasMaxLength(500);
            e.Property(x => x.OriginalFileName).HasMaxLength(300);
            e.Property(x => x.ContentType).HasMaxLength(150);
            e.HasIndex(x => x.LessonId);
        });

        builder.Entity<CourseUnit>(e =>
        {
            e.Property(x => x.TitleAr).HasMaxLength(250).IsRequired();
            e.Property(x => x.TitleEn).HasMaxLength(250);
            e.HasIndex(x => new { x.SchoolId, x.SubjectId });
        });

        var isSqlite = Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var property = builder.Entity(entityType.ClrType).Property(nameof(AuditableEntity.RowVersion));
            if (isSqlite)
            {
                property.IsRequired(false);
            }
            else
            {
                property.IsRowVersion().IsConcurrencyToken();
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var userId = _currentUser?.UserId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.CreatedByUserId ??= userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
                entry.Entity.UpdatedByUserId = userId;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = utcNow;
                entry.Entity.DeletedByUserId = userId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
