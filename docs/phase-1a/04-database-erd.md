# Phase 1A — Database ERD

Logical ERD for Phase 1 design. Physical EF configurations and migrations follow in Phase 2.

Conventions:

- Every school-owned business table includes `SchoolId` (unless platform-global).
- Important tables include audit/soft-delete/`RowVersion` columns (omitted from diagrams for clarity).
- Money fields use `decimal(18,2)`.
- Text that may contain Arabic uses `NVARCHAR`.

## 1. Tenancy, identity, and access

```mermaid
erDiagram
    Schools ||--o{ SchoolBranches : has
    Schools ||--|| SchoolSettings : configures
    Schools ||--o{ AcademicYears : runs
    Schools ||--o{ UserSchoolAssignments : scopes

    AspNetUsers ||--o{ UserSchoolAssignments : assigned
    AspNetUsers ||--o{ UserSchoolRoles : has
    AspNetRoles ||--o{ UserSchoolRoles : grants
    AspNetRoles ||--o{ RolePermissions : includes
    Permissions ||--o{ RolePermissions : granted
    Permissions ||--o{ UserPermissions : overrides
    AspNetUsers ||--o{ UserPermissions : receives
    AspNetUsers ||--o{ LoginHistory : produces
    AspNetUsers ||--o{ AuditLogs : acts

    Schools {
        int SchoolId PK
        nvarchar NameAr
        nvarchar NameEn
        nvarchar LogoPath
        nvarchar Address
        nvarchar Phone
        nvarchar Email
        nvarchar SchoolType
        nvarchar GenderType
        nvarchar Currency
        bit IsActive
    }

    SchoolBranches {
        int SchoolBranchId PK
        int SchoolId FK
        nvarchar NameAr
        nvarchar NameEn
        nvarchar Address
        bit IsActive
    }

    SchoolSettings {
        int SchoolSettingsId PK
        int SchoolId FK
        nvarchar WorkingDaysJson
        nvarchar AttendanceRulesJson
        nvarchar GradeSystemJson
        nvarchar ThemeJson
        nvarchar SmsSettingsJson
        nvarchar NotificationSettingsJson
        nvarchar DateFormat
        nvarchar DefaultCulture
    }

    UserSchoolAssignments {
        int UserSchoolAssignmentId PK
        nvarchar UserId FK
        int SchoolId FK
        int SchoolBranchId FK "nullable"
        bit IsActive
    }

    Permissions {
        int PermissionId PK
        nvarchar Key
        nvarchar NameAr
        nvarchar NameEn
        nvarchar Area
    }

    AuditLogs {
        bigint AuditLogId PK
        nvarchar UserId
        int SchoolId
        nvarchar Action
        nvarchar EntityName
        nvarchar EntityId
        nvarchar OldValues
        nvarchar NewValues
        nvarchar IpAddress
        datetime2 CreatedAt
        bit Success
    }
```

## 2. Academic structure and enrollment

```mermaid
erDiagram
    Schools ||--o{ AcademicYears : has
    AcademicYears ||--o{ Semesters : contains
    Schools ||--o{ AcademicStages : has
    AcademicStages ||--o{ GradeLevels : contains
    GradeLevels ||--o{ ClassSections : contains
    Schools ||--o{ Subjects : offers
    Subjects ||--o{ SubjectGradeAssignments : mapped
    GradeLevels ||--o{ SubjectGradeAssignments : uses
    Teachers ||--o{ TeacherAssignments : teaches
    ClassSections ||--o{ TeacherAssignments : hosts
    Subjects ||--o{ TeacherAssignments : covers
    Students ||--o{ StudentEnrollments : enrolls
    AcademicYears ||--o{ StudentEnrollments : period
    ClassSections ||--o{ StudentEnrollments : placed
    Schools ||--o{ Classrooms : has
    Schools ||--o{ TeachingPeriods : defines
    ClassSections ||--o{ ClassSchedules : scheduled
    TeachingPeriods ||--o{ ClassSchedules : slot
    Subjects ||--o{ ClassSchedules : subject
    Teachers ||--o{ ClassSchedules : teacher
    Classrooms ||--o{ ClassSchedules : room

    AcademicYears {
        int AcademicYearId PK
        int SchoolId FK
        nvarchar NameAr
        date StartDate
        date EndDate
        nvarchar Status
        bit IsCurrent
    }

    StudentEnrollments {
        int StudentEnrollmentId PK
        int SchoolId FK
        int StudentId FK
        int AcademicYearId FK
        int GradeLevelId FK
        int ClassSectionId FK
        nvarchar Status
        int SeatNumber
    }

    ClassSchedules {
        int ClassScheduleId PK
        int SchoolId FK
        int AcademicYearId FK
        int ClassSectionId FK
        int SubjectId FK
        int TeacherId FK
        int ClassroomId FK
        int TeachingPeriodId FK
        tinyint DayOfWeek
    }
```

## 3. Students, guardians, and profiles

```mermaid
erDiagram
    AspNetUsers ||--o| Students : account
    AspNetUsers ||--o| Guardians : account
    Students ||--o{ StudentGuardians : linked
    Guardians ||--o{ StudentGuardians : linked
    Students ||--o{ StudentAddresses : lives
    Students ||--o{ EmergencyContacts : emergency
    Students ||--o| StudentHealthProfiles : health
    Students ||--o| StudentEducationalProfiles : learning
    Students ||--o{ StudentTalents : talents
    Students ||--o{ StudentActivities : activities
    Students ||--o{ StudentDocuments : files
    FileAttachments ||--o{ StudentDocuments : stores

    Students {
        int StudentId PK
        int SchoolId FK
        nvarchar UserId FK
        nvarchar StudentNumber
        nvarchar FullNameAr
        nvarchar FullNameEn
        nvarchar Gender
        date DateOfBirth
        nvarchar NationalId
        nvarchar Status
    }

    Guardians {
        int GuardianId PK
        int SchoolId FK
        nvarchar UserId FK
        nvarchar FullNameAr
        nvarchar Phone
        nvarchar Email
        nvarchar NationalId
    }

    StudentGuardians {
        int StudentGuardianId PK
        int StudentId FK
        int GuardianId FK
        nvarchar Relationship
        bit IsPrimary
        bit IsFinanciallyResponsible
        bit CanReceiveNotifications
        bit CanCollectStudent
    }

    StudentHealthProfiles {
        int StudentHealthProfileId PK
        int StudentId FK
        nvarchar BloodType
        nvarchar ChronicDiseases
        nvarchar Allergies
        nvarchar Medications
        nvarchar EmergencyNotes
    }
```

## 4. LMS: lessons, homework, quizzes

```mermaid
erDiagram
    Subjects ||--o{ CourseUnits : organizes
    CourseUnits ||--o{ Lessons : contains
    Lessons ||--o{ LessonResources : has
    Lessons ||--o{ LessonProgress : tracked
    Students ||--o{ LessonProgress : progresses

    Subjects ||--o{ Assignments : assigns
    Assignments ||--o{ AssignmentTargets : targets
    Assignments ||--o{ AssignmentAttachments : files
    Assignments ||--o{ AssignmentSubmissions : receives
    Students ||--o{ AssignmentSubmissions : submits
    AssignmentSubmissions ||--o{ SubmissionAttachments : files
    AssignmentSubmissions ||--o| AssignmentFeedback : feedback

    Subjects ||--o{ QuestionBanks : owns
    QuestionBanks ||--o{ Questions : contains
    Questions ||--o{ QuestionOptions : options
    Quizzes ||--o{ QuizQuestions : includes
    Questions ||--o{ QuizQuestions : used
    Quizzes ||--o{ QuizAttempts : attempted
    Students ||--o{ QuizAttempts : takes
    QuizAttempts ||--o{ QuizAnswers : answers

    Lessons {
        int LessonId PK
        int SchoolId FK
        int SubjectId FK
        int CourseUnitId FK
        int TeacherId FK
        nvarchar TitleAr
        nvarchar Status
        datetime2 PublishDate
        int SortOrder
    }

    Assignments {
        int AssignmentId PK
        int SchoolId FK
        int SubjectId FK
        int TeacherId FK
        nvarchar TitleAr
        datetime2 DueDate
        decimal TotalMarks
        nvarchar SubmissionType
        nvarchar Status
    }

    Quizzes {
        int QuizId PK
        int SchoolId FK
        int SubjectId FK
        datetime2 StartAt
        datetime2 EndAt
        int DurationMinutes
        int AttemptsAllowed
        decimal PassingMark
        bit RandomizeQuestions
    }
```

## 5. Exams, grades, attendance

```mermaid
erDiagram
    ExamPeriods ||--o{ Exams : groups
    Exams ||--o{ ExamSchedules : scheduled
    Subjects ||--o{ Exams : examined
    GradeCategories ||--o{ GradeItems : contains
    GradeItems ||--o{ StudentGrades : scored
    Students ||--o{ StudentGrades : receives
    StudentGrades ||--o{ GradeApprovals : workflow

    AttendanceSessions ||--o{ StudentAttendance : records
    ClassSections ||--o{ AttendanceSessions : session
    Students ||--o{ StudentAttendance : marked
    StudentAttendance ||--o{ AttendanceExcuses : excuse
    StudentAttendance ||--o{ AttendanceCorrections : correction

    Exams {
        int ExamId PK
        int SchoolId FK
        int ExamPeriodId FK
        int SubjectId FK
        nvarchar ExamType
        decimal MaxScore
        decimal PassScore
        datetime2 ResultPublishAt
    }

    StudentGrades {
        int StudentGradeId PK
        int SchoolId FK
        int StudentId FK
        int GradeItemId FK
        decimal Score
        nvarchar Status
    }

    AttendanceSessions {
        int AttendanceSessionId PK
        int SchoolId FK
        int ClassSectionId FK
        int TeachingPeriodId FK "nullable"
        date AttendanceDate
        nvarchar Status
    }

    StudentAttendance {
        int StudentAttendanceId PK
        int AttendanceSessionId FK
        int StudentId FK
        nvarchar Status
        time LateTime
        nvarchar Notes
    }
```

## 6. Communication, behaviour, badges

```mermaid
erDiagram
    Announcements ||--o{ AnnouncementTargets : targets
    Events ||--o{ EventRegistrations : registrations
    NotificationTemplates ||--o{ Notifications : renders
    Notifications ||--o{ NotificationRecipients : delivers
    AspNetUsers ||--o{ UserDevices : devices

    BehaviourCategories ||--o{ BehaviourRecords : classifies
    Students ||--o{ BehaviourRecords : about
    Students ||--o{ CounsellingSessions : attends
    CounsellingSessions ||--o{ FollowUpPlans : plans

    Badges ||--o{ StudentBadges : awarded
    Students ||--o{ StudentBadges : earns

    Messages ||--o{ MessageRecipients : to
    Messages ||--o{ MessageAttachments : files

    Announcements {
        int AnnouncementId PK
        int SchoolId FK "nullable for global"
        nvarchar TitleAr
        nvarchar Priority
        datetime2 PublishAt
        datetime2 ExpireAt
        bit RequiresAck
        bit IsPinned
    }

    BehaviourRecords {
        int BehaviourRecordId PK
        int SchoolId FK
        int StudentId FK
        int BehaviourCategoryId FK
        date RecordDate
        int Points
        bit IsConfidential
        bit ParentNotified
        nvarchar Status
    }
```

## 7. Finance, transport, library

```mermaid
erDiagram
    FeeTypes ||--o{ FeePlans : typed
    FeePlans ||--o{ StudentFees : assigned
    Students ||--o{ StudentFees : owes
    StudentFees ||--o{ Installments : splits
    Payments ||--o{ PaymentAllocations : allocates
    StudentFees ||--o{ PaymentAllocations : reduces
    StudentFees ||--o{ Discounts : discounted
    Payments ||--o{ Refunds : refunded
    Payments ||--o| Receipts : receipt

    Vehicles ||--o{ TransportRoutes : serves
    TransportRoutes ||--o{ TransportStops : stops
    TransportRoutes ||--o{ StudentTransportAssignments : assigns
    Students ||--o{ StudentTransportAssignments : rides
    Drivers ||--o{ Vehicles : drives

    Books ||--o{ BookCopies : copies
    BookCopies ||--o{ LibraryLoans : loaned
    BookCopies ||--o{ LibraryReservations : reserved
    Students ||--o{ LibraryLoans : borrows

    StudentFees {
        int StudentFeeId PK
        int SchoolId FK
        int StudentId FK
        int FeePlanId FK
        decimal Amount
        decimal RemainingAmount
        nvarchar Status
    }

    Payments {
        int PaymentId PK
        int SchoolId FK
        int StudentId FK
        decimal Amount
        datetime2 PaidAt
        nvarchar Method
        nvarchar Status
    }
```

## 8. Key uniqueness and indexes

### Unique constraints (examples)

| Rule | Constraint idea |
|------|-----------------|
| Student number unique per school | `UQ_Students_SchoolId_StudentNumber` |
| One active enrollment per student per year | filtered unique on `(StudentId, AcademicYearId)` where active |
| One attendance mark per student per session | `UQ_StudentAttendance_Session_Student` |
| One active submission per attempt | `(AssignmentId, StudentId, AttemptNumber)` |
| Permission key unique | `UQ_Permissions_Key` |
| Role name unique | Identity default |

### Index priorities

`SchoolId`, `AcademicYearId`, `StudentId`, `TeacherId`, `Guardian`/`ParentUserId`, `GradeLevelId`, `ClassSectionId`, `SubjectId`, `CreatedAt`, `Status`, `DueDate`, `ExamDate`, `AttendanceDate`.

## 9. Seed data (Phase 2 preview)

- 1 Super Administrator
- 4 sample schools (Arabic names)
- Academic year current + previous
- Stages/grades/sections sample
- Roles + full permission catalog + default role grants
- Demo teacher, student, parent linked to one school
- Sample fee types and badge categories
