using Microsoft.EntityFrameworkCore;
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

namespace SchoolLMS.Application.Common;

public interface IApplicationDbContext
{
    DbSet<School> Schools { get; }
    DbSet<SchoolBranch> SchoolBranches { get; }
    DbSet<SchoolSettings> SchoolSettings { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserPermission> UserPermissions { get; }
    DbSet<UserSchoolAssignment> UserSchoolAssignments { get; }
    DbSet<UserSchoolRole> UserSchoolRoles { get; }
    DbSet<LoginHistory> LoginHistories { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<AcademicYear> AcademicYears { get; }
    DbSet<Semester> Semesters { get; }
    DbSet<AcademicStage> AcademicStages { get; }
    DbSet<GradeLevel> GradeLevels { get; }
    DbSet<ClassSection> ClassSections { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<SubjectGradeAssignment> SubjectGradeAssignments { get; }
    DbSet<Classroom> Classrooms { get; }
    DbSet<TeachingPeriod> TeachingPeriods { get; }
    DbSet<ClassSchedule> ClassSchedules { get; }
    DbSet<RoutineLesson> RoutineLessons { get; }
    DbSet<StudentEnrollment> StudentEnrollments { get; }
    DbSet<TeacherAssignment> TeacherAssignments { get; }

    DbSet<Student> Students { get; }
    DbSet<Guardian> Guardians { get; }
    DbSet<StudentGuardian> StudentGuardians { get; }
    DbSet<EmergencyContact> EmergencyContacts { get; }
    DbSet<StudentAddress> StudentAddresses { get; }
    DbSet<StudentHealthProfile> StudentHealthProfiles { get; }
    DbSet<StudentEducationalProfile> StudentEducationalProfiles { get; }
    DbSet<StudentDocument> StudentDocuments { get; }
    DbSet<StudentHobby> StudentHobbies { get; }
    DbSet<StudentNote> StudentNotes { get; }
    DbSet<StudentQimamCertificate> StudentQimamCertificates { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Teacher> Teachers { get; }

    DbSet<CourseUnit> CourseUnits { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<LessonResource> LessonResources { get; }
    DbSet<LessonClassSection> LessonClassSections { get; }
    DbSet<LessonProgress> LessonProgresses { get; }
    DbSet<Assignment> Assignments { get; }
    DbSet<AssignmentSubmission> AssignmentSubmissions { get; }
    DbSet<Question> Questions { get; }
    DbSet<QuestionOption> QuestionOptions { get; }
    DbSet<Quiz> Quizzes { get; }
    DbSet<QuizQuestion> QuizQuestions { get; }
    DbSet<QuizAttempt> QuizAttempts { get; }

    DbSet<ExamPeriod> ExamPeriods { get; }
    DbSet<Exam> Exams { get; }
    DbSet<GradeCategory> GradeCategories { get; }
    DbSet<GradeItem> GradeItems { get; }
    DbSet<StudentGrade> StudentGrades { get; }
    DbSet<AttendanceSession> AttendanceSessions { get; }
    DbSet<StudentAttendance> StudentAttendances { get; }
    DbSet<AttendanceExcuse> AttendanceExcuses { get; }

    DbSet<Announcement> Announcements { get; }
    DbSet<AnnouncementTarget> AnnouncementTargets { get; }
    DbSet<SchoolEvent> SchoolEvents { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationTemplate> NotificationTemplates { get; }
    DbSet<Message> Messages { get; }
    DbSet<MessageRecipient> MessageRecipients { get; }
    DbSet<MessageAttachment> MessageAttachments { get; }
    DbSet<BehaviourCategory> BehaviourCategories { get; }
    DbSet<BehaviourRecord> BehaviourRecords { get; }
    DbSet<Badge> Badges { get; }
    DbSet<StudentBadge> StudentBadges { get; }

    DbSet<FeeType> FeeTypes { get; }
    DbSet<FeePlan> FeePlans { get; }
    DbSet<StudentFee> StudentFees { get; }
    DbSet<Payment> Payments { get; }
    DbSet<PaymentAllocation> PaymentAllocations { get; }

    DbSet<Vehicle> Vehicles { get; }
    DbSet<TransportRoute> TransportRoutes { get; }
    DbSet<StudentTransportAssignment> StudentTransportAssignments { get; }

    DbSet<Book> Books { get; }
    DbSet<BookCopy> BookCopies { get; }
    DbSet<LibraryLoan> LibraryLoans { get; }
    DbSet<FileAttachment> FileAttachments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
