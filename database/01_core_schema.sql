SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- =========================
-- Lookup / Roles / Users
-- =========================
CREATE TABLE dbo.Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE dbo.Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    RoleID INT NOT NULL,
    Status BIT NOT NULL DEFAULT 1,
    LastLoginDate DATETIME2 NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES dbo.Roles(RoleID)
);
GO

CREATE TABLE dbo.Universities (
    UniversityID INT IDENTITY(1,1) PRIMARY KEY,
    UniversityName NVARCHAR(250) NOT NULL,
    BranchName NVARCHAR(250) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.Students (
    StudentID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    UniversityID INT NULL,
    StudentCode NVARCHAR(50) NOT NULL UNIQUE,
    Department NVARCHAR(200) NULL,
    AcademicYear NVARCHAR(50) NULL,
    EnrollmentDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    AcademicStatus NVARCHAR(50) NOT NULL DEFAULT 'Active',
    CONSTRAINT FK_Students_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID),
    CONSTRAINT FK_Students_Universities FOREIGN KEY (UniversityID) REFERENCES dbo.Universities(UniversityID)
);
GO

CREATE TABLE dbo.Instructors (
    InstructorID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    Specialization NVARCHAR(200) NULL,
    Bio NVARCHAR(MAX) NULL,
    Availability NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Instructors_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID)
);
GO

-- =========================
-- Course / Learning
-- =========================
CREATE TABLE dbo.CourseCategories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.Courses (
    CourseID INT IDENTITY(1,1) PRIMARY KEY,
    CourseTitle NVARCHAR(250) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CategoryID INT NULL,
    LevelName NVARCHAR(100) NULL,
    DurationHours DECIMAL(10,2) NULL,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    InstructorID INT NULL,
    IsPublished BIT NOT NULL DEFAULT 0,
    ApprovalStatus NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Courses_Category FOREIGN KEY (CategoryID) REFERENCES dbo.CourseCategories(CategoryID),
    CONSTRAINT FK_Courses_Instructor FOREIGN KEY (InstructorID) REFERENCES dbo.Instructors(InstructorID)
);
GO

CREATE TABLE dbo.CourseSections (
    SectionID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    SectionTitle NVARCHAR(250) NOT NULL,
    DisplayOrder INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CourseSections_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID)
);
GO

CREATE TABLE dbo.Lessons (
    LessonID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    SectionID INT NULL,
    LessonTitle NVARCHAR(250) NOT NULL,
    VideoURL NVARCHAR(500) NULL,
    DurationMinutes INT NOT NULL DEFAULT 0,
    LearningObjectives NVARCHAR(MAX) NULL,
    DisplayOrder INT NOT NULL DEFAULT 1,
    IsVisible BIT NOT NULL DEFAULT 1,
    PrerequisiteLessonID INT NULL,
    CONSTRAINT FK_Lessons_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_Lessons_Section FOREIGN KEY (SectionID) REFERENCES dbo.CourseSections(SectionID),
    CONSTRAINT FK_Lessons_Prereq FOREIGN KEY (PrerequisiteLessonID) REFERENCES dbo.Lessons(LessonID)
);
GO

CREATE TABLE dbo.LessonResources (
    ResourceID INT IDENTITY(1,1) PRIMARY KEY,
    LessonID INT NOT NULL,
    ResourceType NVARCHAR(50) NOT NULL,
    FileName NVARCHAR(300) NOT NULL,
    FileURL NVARCHAR(500) NOT NULL,
    UploadedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_LessonResources_Lesson FOREIGN KEY (LessonID) REFERENCES dbo.Lessons(LessonID)
);
GO

CREATE TABLE dbo.Enrollments (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    EnrollmentDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CompletionPercentage DECIMAL(5,2) NOT NULL DEFAULT 0,
    EnrollmentStatus NVARCHAR(50) NOT NULL DEFAULT 'Active',
    UNIQUE (StudentID, CourseID),
    CONSTRAINT FK_Enrollments_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_Enrollments_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID)
);
GO

CREATE TABLE dbo.StudentLessonProgress (
    ProgressID BIGINT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    LessonID INT NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    LastViewedAt DATETIME2 NULL,
    StudyMinutes INT NOT NULL DEFAULT 0,
    UNIQUE (StudentID, LessonID),
    CONSTRAINT FK_StudentLessonProgress_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_StudentLessonProgress_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_StudentLessonProgress_Lesson FOREIGN KEY (LessonID) REFERENCES dbo.Lessons(LessonID)
);
GO

-- =========================
-- Class / Live Session
-- =========================
CREATE TABLE dbo.ClassGroups (
    ClassID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    ClassName NVARCHAR(200) NOT NULL,
    StartDate DATE NULL,
    EndDate DATE NULL,
    WeeklySchedule NVARCHAR(300) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
    CONSTRAINT FK_ClassGroups_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID)
);
GO

CREATE TABLE dbo.ClassStudents (
    ClassStudentID BIGINT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT NOT NULL,
    StudentID INT NOT NULL,
    AssignedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UNIQUE (ClassID, StudentID),
    CONSTRAINT FK_ClassStudents_Class FOREIGN KEY (ClassID) REFERENCES dbo.ClassGroups(ClassID),
    CONSTRAINT FK_ClassStudents_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID)
);
GO

CREATE TABLE dbo.LiveClassSessions (
    LiveSessionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    ClassID INT NULL,
    SessionTitle NVARCHAR(250) NOT NULL,
    SessionDescription NVARCHAR(MAX) NULL,
    StartDateTime DATETIME2 NOT NULL,
    DurationMinutes INT NOT NULL,
    MeetingProvider NVARCHAR(50) NULL,
    MeetingLink NVARCHAR(500) NULL,
    SessionStatus NVARCHAR(50) NOT NULL DEFAULT 'Scheduled',
    RecordingURL NVARCHAR(500) NULL,
    CONSTRAINT FK_LiveClassSessions_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_LiveClassSessions_Class FOREIGN KEY (ClassID) REFERENCES dbo.ClassGroups(ClassID)
);
GO

CREATE TABLE dbo.LiveClassAttendance (
    AttendanceID BIGINT IDENTITY(1,1) PRIMARY KEY,
    LiveSessionID BIGINT NOT NULL,
    StudentID INT NOT NULL,
    JoinedAt DATETIME2 NULL,
    LeftAt DATETIME2 NULL,
    AttendanceStatus NVARCHAR(50) NOT NULL DEFAULT 'Absent',
    MarkedByUserID INT NULL,
    UNIQUE (LiveSessionID, StudentID),
    CONSTRAINT FK_LiveClassAttendance_Session FOREIGN KEY (LiveSessionID) REFERENCES dbo.LiveClassSessions(LiveSessionID),
    CONSTRAINT FK_LiveClassAttendance_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_LiveClassAttendance_User FOREIGN KEY (MarkedByUserID) REFERENCES dbo.Users(UserID)
);
GO

-- =========================
-- Assessment / Exam
-- =========================
CREATE TABLE dbo.Assessments (
    AssessmentID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    AssessmentType NVARCHAR(50) NOT NULL,
    TotalMarks DECIMAL(9,2) NOT NULL,
    PassingMarks DECIMAL(9,2) NOT NULL,
    DurationMinutes INT NOT NULL DEFAULT 60,
    StartDateTime DATETIME2 NULL,
    EndDateTime DATETIME2 NULL,
    MaxAttempts INT NOT NULL DEFAULT 1,
    IsAutoGradingEnabled BIT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Assessments_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID)
);
GO

CREATE TABLE dbo.AssessmentQuestions (
    QuestionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    AssessmentID INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionType NVARCHAR(50) NOT NULL,
    Marks DECIMAL(9,2) NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 1,
    IsRequired BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_AssessmentQuestions_Assessment FOREIGN KEY (AssessmentID) REFERENCES dbo.Assessments(AssessmentID)
);
GO

CREATE TABLE dbo.AssessmentQuestionOptions (
    OptionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    QuestionID BIGINT NOT NULL,
    OptionText NVARCHAR(1000) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_AssessmentQuestionOptions_Question FOREIGN KEY (QuestionID) REFERENCES dbo.AssessmentQuestions(QuestionID)
);
GO

CREATE TABLE dbo.ExamCodes (
    ExamCodeID BIGINT IDENTITY(1,1) PRIMARY KEY,
    ExamID INT NOT NULL,
    CourseID INT NOT NULL,
    StudentID INT NULL,
    ExamCode NVARCHAR(100) NOT NULL UNIQUE,
    IsUsed BIT NOT NULL DEFAULT 0,
    ExpiryDate DATETIME2 NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NULL,
    CONSTRAINT FK_ExamCodes_Exam FOREIGN KEY (ExamID) REFERENCES dbo.Assessments(AssessmentID),
    CONSTRAINT FK_ExamCodes_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_ExamCodes_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_ExamCodes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserID)
);
GO

CREATE TABLE dbo.ExamSessions (
    SessionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    ExamID INT NOT NULL,
    ExamCode NVARCHAR(100) NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    DurationMinutes INT NOT NULL DEFAULT 60,
    Score DECIMAL(9,2) NULL,
    PassingScore DECIMAL(9,2) NOT NULL,
    ResultStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    SubmissionStatus NVARCHAR(30) NOT NULL DEFAULT 'Started',
    AttemptNo INT NOT NULL DEFAULT 1,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Active',
    UNIQUE (StudentID, ExamID, AttemptNo),
    CONSTRAINT FK_ExamSessions_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_ExamSessions_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_ExamSessions_Exam FOREIGN KEY (ExamID) REFERENCES dbo.Assessments(AssessmentID)
);
GO

CREATE TABLE dbo.ExamAnswers (
    AnswerID BIGINT IDENTITY(1,1) PRIMARY KEY,
    SessionID BIGINT NOT NULL,
    QuestionID BIGINT NOT NULL,
    StudentID INT NOT NULL,
    SelectedAnswerID BIGINT NULL,
    TextAnswer NVARCHAR(MAX) NULL,
    FileURL NVARCHAR(500) NULL,
    IsCorrect BIT NULL,
    MarkAwarded DECIMAL(9,2) NULL,
    AnsweredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UNIQUE (SessionID, QuestionID),
    CONSTRAINT FK_ExamAnswers_Session FOREIGN KEY (SessionID) REFERENCES dbo.ExamSessions(SessionID),
    CONSTRAINT FK_ExamAnswers_Question FOREIGN KEY (QuestionID) REFERENCES dbo.AssessmentQuestions(QuestionID),
    CONSTRAINT FK_ExamAnswers_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_ExamAnswers_Option FOREIGN KEY (SelectedAnswerID) REFERENCES dbo.AssessmentQuestionOptions(OptionID)
);
GO

-- =========================
-- Survey & Certificates
-- =========================
CREATE TABLE dbo.CertificateSurveys (
    SurveyID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    ExamID INT NOT NULL,
    SurveyTitle NVARCHAR(250) NOT NULL,
    IsRequiredForCertificate BIT NOT NULL DEFAULT 1,
    Status BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CertificateSurveys_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_CertificateSurveys_Exam FOREIGN KEY (ExamID) REFERENCES dbo.Assessments(AssessmentID)
);
GO

CREATE TABLE dbo.CertificateSurveyQuestions (
    SurveyQuestionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    SurveyID INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionType NVARCHAR(20) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CertificateSurveyQuestions_Survey FOREIGN KEY (SurveyID) REFERENCES dbo.CertificateSurveys(SurveyID)
);
GO

CREATE TABLE dbo.CertificateSurveyResponses (
    ResponseID BIGINT IDENTITY(1,1) PRIMARY KEY,
    SurveyID INT NOT NULL,
    SurveyQuestionID BIGINT NOT NULL,
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    ExamSessionID BIGINT NOT NULL,
    ResponseValue NVARCHAR(MAX) NOT NULL,
    SubmittedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_CertificateSurveyResponses_Survey FOREIGN KEY (SurveyID) REFERENCES dbo.CertificateSurveys(SurveyID),
    CONSTRAINT FK_CertificateSurveyResponses_Question FOREIGN KEY (SurveyQuestionID) REFERENCES dbo.CertificateSurveyQuestions(SurveyQuestionID),
    CONSTRAINT FK_CertificateSurveyResponses_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_CertificateSurveyResponses_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_CertificateSurveyResponses_Session FOREIGN KEY (ExamSessionID) REFERENCES dbo.ExamSessions(SessionID)
);
GO

CREATE TABLE dbo.Certificates (
    CertificateID BIGINT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    ExamSessionID BIGINT NOT NULL,
    SurveyCompleted BIT NOT NULL DEFAULT 0,
    IssueDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    QRCodeURL NVARCHAR(500) NULL,
    CertificateNumber NVARCHAR(100) NOT NULL UNIQUE,
    CertificatePDFURL NVARCHAR(500) NULL,
    CertificateStatus NVARCHAR(20) NOT NULL DEFAULT 'Valid',
    UNIQUE (StudentID, CourseID, ExamSessionID),
    CONSTRAINT FK_Certificates_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_Certificates_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_Certificates_Session FOREIGN KEY (ExamSessionID) REFERENCES dbo.ExamSessions(SessionID)
);
GO

-- =========================
-- Finance
-- =========================
CREATE TABLE dbo.SubscriptionPlans (
    PlanID INT IDENTITY(1,1) PRIMARY KEY,
    PlanName NVARCHAR(200) NOT NULL,
    PlanPrice DECIMAL(18,2) NOT NULL,
    DurationMonths INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.Payments (
    PaymentID BIGINT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    EnrollmentID INT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(30) NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    InvoiceNumber NVARCHAR(100) NULL,
    TransactionReference NVARCHAR(150) NULL,
    Remarks NVARCHAR(500) NULL,
    CONSTRAINT FK_Payments_Student FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID),
    CONSTRAINT FK_Payments_Enrollment FOREIGN KEY (EnrollmentID) REFERENCES dbo.Enrollments(EnrollmentID)
);
GO

-- =========================
-- Notifications / Announcements
-- =========================
CREATE TABLE dbo.Announcements (
    AnnouncementID BIGINT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(250) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    AudienceRole NVARCHAR(50) NULL,
    CourseID INT NULL,
    ClassID INT NULL,
    IsPublished BIT NOT NULL DEFAULT 1,
    PublishedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NULL,
    CONSTRAINT FK_Announcements_Course FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID),
    CONSTRAINT FK_Announcements_Class FOREIGN KEY (ClassID) REFERENCES dbo.ClassGroups(ClassID),
    CONSTRAINT FK_Announcements_User FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserID)
);
GO

-- =========================
-- Helpful Indexes
-- =========================
CREATE INDEX IX_Enrollments_Student ON dbo.Enrollments(StudentID, EnrollmentStatus);
CREATE INDEX IX_Enrollments_Course ON dbo.Enrollments(CourseID);
CREATE INDEX IX_ExamSessions_StudentExam ON dbo.ExamSessions(StudentID, ExamID, ResultStatus);
CREATE INDEX IX_ExamAnswers_Session ON dbo.ExamAnswers(SessionID);
CREATE INDEX IX_Certificates_Student ON dbo.Certificates(StudentID, CertificateStatus);
CREATE INDEX IX_Payments_StudentDate ON dbo.Payments(StudentID, PaymentDate DESC);
CREATE INDEX IX_LiveClassSessions_CourseDate ON dbo.LiveClassSessions(CourseID, StartDateTime);
GO

-- =========================
-- Seed base roles
-- =========================
INSERT INTO dbo.Roles(RoleName)
VALUES
    ('Admin'),
    ('Student'),
    ('Instructor'),
    ('Finance Officer'),
    ('Exam Officer'),
    ('Viewer');
GO
