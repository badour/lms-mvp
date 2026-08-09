IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AcademicStages] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(200) NOT NULL,
        [NameEn] nvarchar(200) NOT NULL,
        [YearName] nvarchar(100) NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_AcademicStages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AcademicYears] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [Status] int NOT NULL,
        [IsCurrent] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_AcademicYears] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Announcements] (
        [Id] int NOT NULL IDENTITY,
        [SchoolId] int NULL,
        [TitleAr] nvarchar(max) NOT NULL,
        [TitleEn] nvarchar(max) NULL,
        [ContentAr] nvarchar(max) NOT NULL,
        [ContentEn] nvarchar(max) NULL,
        [ImagePath] nvarchar(max) NULL,
        [Priority] int NOT NULL,
        [PublishAt] datetime2 NOT NULL,
        [ExpireAt] datetime2 NULL,
        [AuthorUserId] nvarchar(max) NULL,
        [RequiresAcknowledgement] bit NOT NULL,
        [IsPinned] bit NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Announcements] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(128) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(128) NOT NULL,
        [FullNameAr] nvarchar(max) NOT NULL,
        [FullNameEn] nvarchar(max) NULL,
        [ProfileImagePath] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [MustChangePassword] bit NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [PreferredCulture] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Assignments] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SemesterId] int NULL,
        [SubjectId] int NOT NULL,
        [TeacherId] int NOT NULL,
        [GradeLevelId] int NULL,
        [ClassSectionId] int NULL,
        [TitleAr] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [AssignedAt] datetime2 NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [TotalMarks] decimal(18,2) NOT NULL,
        [SubmissionType] int NOT NULL,
        [AllowLateSubmission] bit NOT NULL,
        [LatePenaltyPercent] decimal(18,2) NULL,
        [Status] int NOT NULL,
        [Instructions] nvarchar(max) NULL,
        [AttachmentPath] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AttendanceExcuses] (
        [Id] int NOT NULL IDENTITY,
        [StudentAttendanceId] int NOT NULL,
        [Reason] nvarchar(max) NOT NULL,
        [AttachmentPath] nvarchar(max) NULL,
        [Status] nvarchar(max) NOT NULL,
        [ReviewedByUserId] nvarchar(max) NULL,
        [ReviewedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_AttendanceExcuses] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AttendanceSessions] (
        [Id] int NOT NULL IDENTITY,
        [ClassSectionId] int NOT NULL,
        [TeachingPeriodId] int NULL,
        [TeacherId] int NULL,
        [AttendanceDate] date NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_AttendanceSessions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [UserName] nvarchar(max) NULL,
        [SchoolId] int NULL,
        [Action] nvarchar(max) NOT NULL,
        [EntityName] nvarchar(max) NULL,
        [EntityId] nvarchar(max) NULL,
        [OldValues] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [Browser] nvarchar(max) NULL,
        [Device] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [Success] bit NOT NULL,
        [FailureReason] nvarchar(max) NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Badges] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Icon] nvarchar(max) NULL,
        [ImagePath] nvarchar(max) NULL,
        [Category] nvarchar(max) NOT NULL,
        [Points] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Badges] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [BehaviourCategories] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Polarity] int NOT NULL,
        [DefaultPoints] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_BehaviourCategories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Books] (
        [Id] int NOT NULL IDENTITY,
        [TitleAr] nvarchar(max) NOT NULL,
        [TitleEn] nvarchar(max) NULL,
        [Author] nvarchar(max) NULL,
        [Publisher] nvarchar(max) NULL,
        [Category] nvarchar(max) NULL,
        [Isbn] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Classrooms] (
        [Id] int NOT NULL IDENTITY,
        [SchoolBranchId] int NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Capacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Classrooms] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [CourseUnits] (
        [Id] int NOT NULL IDENTITY,
        [SubjectId] int NOT NULL,
        [GradeLevelId] int NULL,
        [TitleAr] nvarchar(250) NOT NULL,
        [TitleEn] nvarchar(250) NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_CourseUnits] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [FullNameAr] nvarchar(max) NOT NULL,
        [FullNameEn] nvarchar(max) NULL,
        [EmployeeNumber] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [JobTitle] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [ExamPeriods] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_ExamPeriods] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [FeeTypes] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_FeeTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [FileAttachments] (
        [Id] bigint NOT NULL IDENTITY,
        [SchoolId] int NULL,
        [OwnerType] nvarchar(max) NOT NULL,
        [OwnerId] nvarchar(max) NOT NULL,
        [OriginalFileName] nvarchar(max) NOT NULL,
        [StoredFileName] nvarchar(max) NOT NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [RelativePath] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_FileAttachments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [GradeCategories] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [WeightPercent] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_GradeCategories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Guardians] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [FullNameAr] nvarchar(max) NOT NULL,
        [FullNameEn] nvarchar(max) NULL,
        [NationalId] nvarchar(max) NULL,
        [Phone] nvarchar(max) NOT NULL,
        [AlternativePhone] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Occupation] nvarchar(max) NULL,
        [Workplace] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [EducationLevel] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Guardians] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [LoginHistories] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [UserName] nvarchar(max) NULL,
        [SchoolId] int NULL,
        [Success] bit NOT NULL,
        [FailureReason] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        [UserAgent] nvarchar(max) NULL,
        [DeviceInfo] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_LoginHistories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] bigint NOT NULL IDENTITY,
        [SchoolId] int NULL,
        [UserId] nvarchar(max) NOT NULL,
        [TitleAr] nvarchar(max) NOT NULL,
        [BodyAr] nvarchar(max) NOT NULL,
        [TitleEn] nvarchar(max) NULL,
        [BodyEn] nvarchar(max) NULL,
        [Category] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [ReadAt] datetime2 NULL,
        [LinkUrl] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [NotificationTemplates] (
        [Id] int NOT NULL IDENTITY,
        [Key] nvarchar(max) NOT NULL,
        [TitleAr] nvarchar(max) NOT NULL,
        [BodyAr] nvarchar(max) NOT NULL,
        [TitleEn] nvarchar(max) NULL,
        [BodyEn] nvarchar(max) NULL,
        [Channel] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_NotificationTemplates] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaidAt] datetime2 NOT NULL,
        [Method] nvarchar(max) NOT NULL,
        [ReferenceNumber] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [ReceivedByUserId] nvarchar(max) NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Permissions] (
        [Id] int NOT NULL IDENTITY,
        [Key] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Area] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Questions] (
        [Id] int NOT NULL IDENTITY,
        [SubjectId] int NOT NULL,
        [CourseUnitId] int NULL,
        [Topic] nvarchar(max) NULL,
        [Difficulty] nvarchar(max) NOT NULL,
        [QuestionType] int NOT NULL,
        [StemAr] nvarchar(max) NOT NULL,
        [StemEn] nvarchar(max) NULL,
        [DefaultMarks] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Quizzes] (
        [Id] int NOT NULL IDENTITY,
        [SubjectId] int NOT NULL,
        [TeacherId] int NULL,
        [ClassSectionId] int NULL,
        [TitleAr] nvarchar(max) NOT NULL,
        [StartAt] datetime2 NOT NULL,
        [EndAt] datetime2 NOT NULL,
        [DurationMinutes] int NOT NULL,
        [AttemptsAllowed] int NOT NULL,
        [RandomizeQuestions] bit NOT NULL,
        [RandomizeAnswers] bit NOT NULL,
        [QuestionsPerAttempt] int NULL,
        [PassingMark] decimal(18,2) NOT NULL,
        [ShowResultImmediately] bit NOT NULL,
        [ShowCorrectAnswers] bit NOT NULL,
        [NegativeMarking] bit NOT NULL,
        [AccessPassword] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Quizzes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [SchoolEvents] (
        [Id] int NOT NULL IDENTITY,
        [TitleAr] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [StartsAt] datetime2 NOT NULL,
        [EndsAt] datetime2 NULL,
        [Location] nvarchar(max) NULL,
        [Organizer] nvarchar(max) NULL,
        [RequiresRegistration] bit NOT NULL,
        [Capacity] int NULL,
        [IsPublished] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_SchoolEvents] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Schools] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(200) NOT NULL,
        [NameEn] nvarchar(200) NOT NULL,
        [LogoPath] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [SecondaryPhone] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [SchoolType] nvarchar(max) NOT NULL,
        [GenderType] nvarchar(max) NOT NULL,
        [Currency] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Schools] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Students] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [SchoolBranchId] int NULL,
        [StudentNumber] nvarchar(50) NOT NULL,
        [FullNameAr] nvarchar(200) NOT NULL,
        [FullNameEn] nvarchar(max) NULL,
        [FatherName] nvarchar(200) NULL,
        [MotherName] nvarchar(200) NULL,
        [ProfileImagePath] nvarchar(max) NULL,
        [Gender] int NOT NULL,
        [DateOfBirth] date NULL,
        [PlaceOfBirth] nvarchar(max) NULL,
        [Nationality] nvarchar(max) NULL,
        [NationalId] nvarchar(max) NULL,
        [PassportOrCardId] nvarchar(100) NULL,
        [RegistrationDate] date NULL,
        [AdmissionDate] date NULL,
        [ClassClassification] nvarchar(100) NULL,
        [Phone1] nvarchar(30) NULL,
        [Phone2] nvarchar(30) NULL,
        [City] nvarchar(100) NULL,
        [Region] nvarchar(100) NULL,
        [Address] nvarchar(500) NULL,
        [Status] int NOT NULL,
        [PreviousSchool] nvarchar(max) NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Subjects] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Code] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [TeacherAssignments] (
        [Id] int NOT NULL IDENTITY,
        [TeacherId] int NOT NULL,
        [AcademicYearId] int NOT NULL,
        [SubjectId] int NOT NULL,
        [ClassSectionId] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_TeacherAssignments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [TeachingPeriods] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [SortOrder] int NOT NULL,
        [IsBreak] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_TeachingPeriods] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [UserSchoolRoles] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [SchoolId] int NOT NULL,
        [RoleId] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_UserSchoolRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Vehicles] (
        [Id] int NOT NULL IDENTITY,
        [PlateNumber] nvarchar(max) NOT NULL,
        [Model] nvarchar(max) NULL,
        [Capacity] int NOT NULL,
        [DriverName] nvarchar(max) NULL,
        [DriverPhone] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [GradeLevels] (
        [Id] int NOT NULL IDENTITY,
        [AcademicStageId] int NOT NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [SortOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_GradeLevels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GradeLevels_AcademicStages_AcademicStageId] FOREIGN KEY ([AcademicStageId]) REFERENCES [AcademicStages] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Semesters] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NOT NULL,
        [IsCurrent] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Semesters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Semesters_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AnnouncementTargets] (
        [Id] int NOT NULL IDENTITY,
        [AnnouncementId] int NOT NULL,
        [TargetType] nvarchar(max) NOT NULL,
        [TargetValue] nvarchar(max) NULL,
        [GradeLevelId] int NULL,
        [ClassSectionId] int NULL,
        CONSTRAINT [PK_AnnouncementTargets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnnouncementTargets_Announcements_AnnouncementId] FOREIGN KEY ([AnnouncementId]) REFERENCES [Announcements] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(128) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(128) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(128) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(128) NOT NULL,
        [RoleId] nvarchar(128) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(128) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(128) NOT NULL,
        [Token] nvarchar(450) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [ReplacedByToken] nvarchar(max) NULL,
        [CreatedByIp] nvarchar(max) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [AssignmentSubmissions] (
        [Id] int NOT NULL IDENTITY,
        [AssignmentId] int NOT NULL,
        [StudentId] int NOT NULL,
        [AttemptNumber] int NOT NULL,
        [TextResponse] nvarchar(max) NULL,
        [LinkUrl] nvarchar(max) NULL,
        [FilePath] nvarchar(max) NULL,
        [SubmittedAt] datetime2 NULL,
        [Status] int NOT NULL,
        [Score] decimal(18,2) NULL,
        [TeacherFeedback] nvarchar(max) NULL,
        [ReviewedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_AssignmentSubmissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssignmentSubmissions_Assignments_AssignmentId] FOREIGN KEY ([AssignmentId]) REFERENCES [Assignments] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentAttendances] (
        [Id] int NOT NULL IDENTITY,
        [AttendanceSessionId] int NOT NULL,
        [StudentId] int NOT NULL,
        [Status] int NOT NULL,
        [LateTime] time NULL,
        [LeftEarlyTime] time NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentAttendances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentAttendances_AttendanceSessions_AttendanceSessionId] FOREIGN KEY ([AttendanceSessionId]) REFERENCES [AttendanceSessions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentBadges] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [BadgeId] int NOT NULL,
        [AcademicYearId] int NOT NULL,
        [AwardedAt] datetime2 NOT NULL,
        [AwardedByUserId] nvarchar(max) NOT NULL,
        [Reason] nvarchar(max) NULL,
        [VisibleToParent] bit NOT NULL,
        [VisibleToStudent] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentBadges] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentBadges_Badges_BadgeId] FOREIGN KEY ([BadgeId]) REFERENCES [Badges] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [BehaviourRecords] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [BehaviourCategoryId] int NOT NULL,
        [RecordDate] date NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Location] nvarchar(max) NULL,
        [ReportedByUserId] nvarchar(max) NOT NULL,
        [ActionTaken] nvarchar(max) NULL,
        [Points] int NOT NULL,
        [ParentNotified] bit NOT NULL,
        [IsConfidential] bit NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [FollowUpDate] date NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_BehaviourRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BehaviourRecords_BehaviourCategories_BehaviourCategoryId] FOREIGN KEY ([BehaviourCategoryId]) REFERENCES [BehaviourCategories] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [BookCopies] (
        [Id] int NOT NULL IDENTITY,
        [BookId] int NOT NULL,
        [Barcode] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_BookCopies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookCopies_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Teachers] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NULL,
        [EmployeeId] int NULL,
        [FullNameAr] nvarchar(200) NOT NULL,
        [FullNameEn] nvarchar(200) NULL,
        [DocumentId] nvarchar(100) NULL,
        [ParentName] nvarchar(200) NULL,
        [MotherName] nvarchar(200) NULL,
        [Gender] int NOT NULL,
        [MaritalStatus] int NOT NULL,
        [RoleName] nvarchar(100) NOT NULL,
        [Specialization] nvarchar(200) NULL,
        [EducationalInfo] nvarchar(4000) NULL,
        [Phone] nvarchar(30) NULL,
        [Email] nvarchar(200) NULL,
        [City] nvarchar(100) NULL,
        [Address] nvarchar(500) NULL,
        [StartDate] date NULL,
        [EndDate] date NULL,
        [AttachmentPath] nvarchar(500) NULL,
        [AttachmentOriginalName] nvarchar(300) NULL,
        [AttachmentContentType] nvarchar(150) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Teachers_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Exams] (
        [Id] int NOT NULL IDENTITY,
        [ExamPeriodId] int NOT NULL,
        [SubjectId] int NOT NULL,
        [TeacherId] int NULL,
        [GradeLevelId] int NULL,
        [AcademicStageId] int NULL,
        [ClassSectionId] int NULL,
        [ExamType] int NOT NULL,
        [ExamDate] date NOT NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [ClassroomId] int NULL,
        [MaxScore] decimal(18,2) NOT NULL,
        [PassScore] decimal(18,2) NOT NULL,
        [Topics] nvarchar(2000) NULL,
        [Notes] nvarchar(2000) NULL,
        [Instructions] nvarchar(4000) NULL,
        [ResultPublishAt] datetime2 NULL,
        [Status] int NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Exams] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Exams_ExamPeriods_ExamPeriodId] FOREIGN KEY ([ExamPeriodId]) REFERENCES [ExamPeriods] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [FeePlans] (
        [Id] int NOT NULL IDENTITY,
        [FeeTypeId] int NOT NULL,
        [AcademicYearId] int NOT NULL,
        [GradeLevelId] int NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_FeePlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FeePlans_FeeTypes_FeeTypeId] FOREIGN KEY ([FeeTypeId]) REFERENCES [FeeTypes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [GradeItems] (
        [Id] int NOT NULL IDENTITY,
        [GradeCategoryId] int NOT NULL,
        [AcademicYearId] int NOT NULL,
        [SubjectId] int NOT NULL,
        [ClassSectionId] int NULL,
        [TitleAr] nvarchar(max) NOT NULL,
        [MaxScore] decimal(18,2) NOT NULL,
        [DueDate] date NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_GradeItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GradeItems_GradeCategories_GradeCategoryId] FOREIGN KEY ([GradeCategoryId]) REFERENCES [GradeCategories] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [RoleId] nvarchar(128) NOT NULL,
        [PermissionId] int NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [UserPermissions] (
        [UserId] nvarchar(128) NOT NULL,
        [PermissionId] int NOT NULL,
        [IsGranted] bit NOT NULL,
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([UserId], [PermissionId]),
        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [QuestionOptions] (
        [Id] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        [TextAr] nvarchar(max) NOT NULL,
        [IsCorrect] bit NOT NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_QuestionOptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionOptions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [QuizAttempts] (
        [Id] int NOT NULL IDENTITY,
        [QuizId] int NOT NULL,
        [StudentId] int NOT NULL,
        [AttemptNumber] int NOT NULL,
        [StartedAt] datetime2 NOT NULL,
        [SubmittedAt] datetime2 NULL,
        [DurationUsedSeconds] int NULL,
        [Score] decimal(18,2) NULL,
        [IpAddress] nvarchar(max) NULL,
        [DeviceInfo] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_QuizAttempts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuizAttempts_Quizzes_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [Quizzes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [QuizQuestions] (
        [QuizId] int NOT NULL,
        [QuestionId] int NOT NULL,
        [Marks] decimal(18,2) NOT NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_QuizQuestions] PRIMARY KEY ([QuizId], [QuestionId]),
        CONSTRAINT [FK_QuizQuestions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuizQuestions_Quizzes_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [Quizzes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [SchoolBranches] (
        [Id] int NOT NULL IDENTITY,
        [NameAr] nvarchar(450) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NULL,
        [Phone] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_SchoolBranches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SchoolBranches_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [SchoolSettings] (
        [Id] int NOT NULL IDENTITY,
        [WorkingDaysJson] nvarchar(max) NOT NULL,
        [AttendanceRulesJson] nvarchar(max) NOT NULL,
        [GradeSystemJson] nvarchar(max) NOT NULL,
        [ThemeJson] nvarchar(max) NOT NULL,
        [SmsSettingsJson] nvarchar(max) NOT NULL,
        [NotificationSettingsJson] nvarchar(max) NOT NULL,
        [DateFormat] nvarchar(max) NOT NULL,
        [DefaultCulture] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_SchoolSettings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SchoolSettings_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [EmergencyContacts] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [Relationship] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [AlternativePhone] nvarchar(max) NULL,
        [PriorityOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_EmergencyContacts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EmergencyContacts_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentAddresses] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [Country] nvarchar(max) NULL,
        [Governorate] nvarchar(max) NULL,
        [District] nvarchar(max) NULL,
        [Area] nvarchar(max) NULL,
        [Street] nvarchar(max) NULL,
        [NearestLandmark] nvarchar(max) NULL,
        [DetailedAddress] nvarchar(max) NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentAddresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentAddresses_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentDocuments] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [DocumentType] nvarchar(max) NOT NULL,
        [FileName] nvarchar(max) NOT NULL,
        [StoredFileName] nvarchar(max) NOT NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [FileSizeBytes] bigint NOT NULL,
        [RelativePath] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentDocuments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentEducationalProfiles] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [LearningDifficulties] nvarchar(max) NULL,
        [Strengths] nvarchar(max) NULL,
        [Weaknesses] nvarchar(max) NULL,
        [BehaviourNotes] nvarchar(max) NULL,
        [CounsellorRecommendations] nvarchar(max) NULL,
        [SpecialEducationalNeeds] nvarchar(max) NULL,
        [InterventionPlans] nvarchar(max) NULL,
        [FollowUpRecords] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentEducationalProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentEducationalProfiles_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentGuardians] (
        [StudentId] int NOT NULL,
        [GuardianId] int NOT NULL,
        [Relationship] nvarchar(max) NOT NULL,
        [IsPrimary] bit NOT NULL,
        [IsFinanciallyResponsible] bit NOT NULL,
        [CanReceiveNotifications] bit NOT NULL,
        [CanCollectStudent] bit NOT NULL,
        CONSTRAINT [PK_StudentGuardians] PRIMARY KEY ([StudentId], [GuardianId]),
        CONSTRAINT [FK_StudentGuardians_Guardians_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentGuardians_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentHealthProfiles] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [BloodType] nvarchar(max) NULL,
        [ChronicDiseases] nvarchar(max) NULL,
        [Allergies] nvarchar(max) NULL,
        [CurrentMedications] nvarchar(max) NULL,
        [DisabilityInfo] nvarchar(max) NULL,
        [SpecialInstructions] nvarchar(max) NULL,
        [MedicalExamDate] date NULL,
        [DentalExamNotes] nvarchar(max) NULL,
        [VaccinationNotes] nvarchar(max) NULL,
        [EmergencyHealthNotes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentHealthProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentHealthProfiles_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentHobbies] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [SortOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentHobbies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentHobbies_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentNotes] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [NoteText] nvarchar(2000) NOT NULL,
        [NoteDate] datetime2 NOT NULL,
        [SortOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentNotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentNotes_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentQimamCertificates] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [CertificateName] nvarchar(250) NOT NULL,
        [CertificateDate] date NOT NULL,
        [ClassName] nvarchar(150) NOT NULL,
        [ImagePath] nvarchar(500) NULL,
        [ImageOriginalName] nvarchar(300) NULL,
        [DocumentPath] nvarchar(500) NULL,
        [DocumentOriginalName] nvarchar(300) NULL,
        [Notes] nvarchar(2000) NULL,
        [Description] nvarchar(4000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentQimamCertificates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentQimamCertificates_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [TransportRoutes] (
        [Id] int NOT NULL IDENTITY,
        [VehicleId] int NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_TransportRoutes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TransportRoutes_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [ClassSections] (
        [Id] int NOT NULL IDENTITY,
        [GradeLevelId] int NOT NULL,
        [SchoolBranchId] int NULL,
        [NameAr] nvarchar(max) NOT NULL,
        [NameEn] nvarchar(max) NOT NULL,
        [Capacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_ClassSections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClassSections_GradeLevels_GradeLevelId] FOREIGN KEY ([GradeLevelId]) REFERENCES [GradeLevels] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [RoutineLessons] (
        [Id] int NOT NULL IDENTITY,
        [LessonName] nvarchar(200) NOT NULL,
        [GradeLevelId] int NOT NULL,
        [SessionsPerYear] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_RoutineLessons] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoutineLessons_GradeLevels_GradeLevelId] FOREIGN KEY ([GradeLevelId]) REFERENCES [GradeLevels] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [SubjectGradeAssignments] (
        [Id] int NOT NULL IDENTITY,
        [SubjectId] int NOT NULL,
        [GradeLevelId] int NOT NULL,
        [WeeklyPeriods] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_SubjectGradeAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubjectGradeAssignments_GradeLevels_GradeLevelId] FOREIGN KEY ([GradeLevelId]) REFERENCES [GradeLevels] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SubjectGradeAssignments_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [LibraryLoans] (
        [Id] int NOT NULL IDENTITY,
        [BookCopyId] int NOT NULL,
        [StudentId] int NOT NULL,
        [BorrowedOn] date NOT NULL,
        [DueOn] date NOT NULL,
        [ReturnedOn] date NULL,
        [PenaltyAmount] decimal(18,2) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_LibraryLoans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LibraryLoans_BookCopies_BookCopyId] FOREIGN KEY ([BookCopyId]) REFERENCES [BookCopies] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Lessons] (
        [Id] int NOT NULL IDENTITY,
        [SubjectId] int NOT NULL,
        [CourseUnitId] int NOT NULL,
        [TeacherId] int NULL,
        [GradeLevelId] int NULL,
        [ClassSectionId] int NULL,
        [TitleAr] nvarchar(250) NOT NULL,
        [TitleEn] nvarchar(250) NULL,
        [Description] nvarchar(4000) NULL,
        [LearningObjectives] nvarchar(2000) NULL,
        [SortOrder] int NOT NULL,
        [Status] int NOT NULL,
        [IsPosted] bit NOT NULL,
        [PublishDate] datetime2 NULL,
        [ExpiryDate] datetime2 NULL,
        [LessonDateTime] datetime2 NULL,
        [EstimatedDurationMinutes] int NULL,
        [FeaturedImagePath] nvarchar(500) NULL,
        [VideoUrl] nvarchar(1000) NULL,
        [VideoPath] nvarchar(500) NULL,
        [VideoOriginalName] nvarchar(300) NULL,
        [TeacherNotes] nvarchar(2000) NULL,
        [Notes] nvarchar(2000) NULL,
        [StudentInstructions] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Lessons] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Lessons_CourseUnits_CourseUnitId] FOREIGN KEY ([CourseUnitId]) REFERENCES [CourseUnits] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Lessons_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Lessons_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [Messages] (
        [Id] int NOT NULL IDENTITY,
        [SenderUserId] nvarchar(450) NOT NULL,
        [SenderDisplayName] nvarchar(200) NULL,
        [StudentId] int NULL,
        [ParentMessageId] int NULL,
        [Subject] nvarchar(250) NOT NULL,
        [Body] nvarchar(4000) NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [TargetType] int NOT NULL,
        [TeacherId] int NULL,
        [TargetDisplayName] nvarchar(200) NULL,
        [IsArchived] bit NOT NULL,
        [ReplyBody] nvarchar(4000) NULL,
        [RepliedAt] datetime2 NULL,
        [RepliedByUserId] nvarchar(450) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_Messages_ParentMessageId] FOREIGN KEY ([ParentMessageId]) REFERENCES [Messages] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Messages_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Messages_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentFees] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [FeePlanId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [RemainingAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentFees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentFees_FeePlans_FeePlanId] FOREIGN KEY ([FeePlanId]) REFERENCES [FeePlans] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentGrades] (
        [Id] int NOT NULL IDENTITY,
        [GradeItemId] int NOT NULL,
        [StudentId] int NOT NULL,
        [Score] decimal(18,2) NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentGrades] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentGrades_GradeItems_GradeItemId] FOREIGN KEY ([GradeItemId]) REFERENCES [GradeItems] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [UserSchoolAssignments] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(128) NOT NULL,
        [SchoolId] int NOT NULL,
        [SchoolBranchId] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_UserSchoolAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserSchoolAssignments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserSchoolAssignments_SchoolBranches_SchoolBranchId] FOREIGN KEY ([SchoolBranchId]) REFERENCES [SchoolBranches] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserSchoolAssignments_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentTransportAssignments] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [TransportRouteId] int NOT NULL,
        [PickupLocation] nvarchar(max) NULL,
        [ExpectedPickupTime] time NULL,
        [ExpectedDropoffTime] time NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentTransportAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentTransportAssignments_TransportRoutes_TransportRouteId] FOREIGN KEY ([TransportRouteId]) REFERENCES [TransportRoutes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [ClassSchedules] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [ClassSectionId] int NOT NULL,
        [SubjectId] int NULL,
        [TeacherId] int NULL,
        [ClassroomId] int NULL,
        [TeachingPeriodId] int NOT NULL,
        [DayOfWeek] tinyint NOT NULL,
        [EntryText] nvarchar(200) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_ClassSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClassSchedules_ClassSections_ClassSectionId] FOREIGN KEY ([ClassSectionId]) REFERENCES [ClassSections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClassSchedules_Classrooms_ClassroomId] FOREIGN KEY ([ClassroomId]) REFERENCES [Classrooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClassSchedules_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClassSchedules_TeachingPeriods_TeachingPeriodId] FOREIGN KEY ([TeachingPeriodId]) REFERENCES [TeachingPeriods] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [StudentEnrollments] (
        [Id] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [AcademicYearId] int NOT NULL,
        [GradeLevelId] int NOT NULL,
        [ClassSectionId] int NOT NULL,
        [Status] int NOT NULL,
        [SeatNumber] int NULL,
        [SequenceInClass] int NULL,
        [EnrollmentDate] date NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_StudentEnrollments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudentEnrollments_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentEnrollments_ClassSections_ClassSectionId] FOREIGN KEY ([ClassSectionId]) REFERENCES [ClassSections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StudentEnrollments_GradeLevels_GradeLevelId] FOREIGN KEY ([GradeLevelId]) REFERENCES [GradeLevels] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [LessonClassSections] (
        [LessonId] int NOT NULL,
        [ClassSectionId] int NOT NULL,
        CONSTRAINT [PK_LessonClassSections] PRIMARY KEY ([LessonId], [ClassSectionId]),
        CONSTRAINT [FK_LessonClassSections_ClassSections_ClassSectionId] FOREIGN KEY ([ClassSectionId]) REFERENCES [ClassSections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LessonClassSections_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [LessonProgresses] (
        [Id] int NOT NULL IDENTITY,
        [LessonId] int NOT NULL,
        [StudentId] int NOT NULL,
        [Opened] bit NOT NULL,
        [VideoStarted] bit NOT NULL,
        [VideoCompletionPercent] decimal(5,2) NOT NULL,
        [FilesDownloaded] bit NOT NULL,
        [ActivityCompleted] bit NOT NULL,
        [IsCompleted] bit NOT NULL,
        [CompletedAt] datetime2 NULL,
        [TimeSpentSeconds] int NOT NULL,
        [LastAccessedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_LessonProgresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LessonProgresses_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [LessonResources] (
        [Id] int NOT NULL IDENTITY,
        [LessonId] int NOT NULL,
        [ResourceType] nvarchar(50) NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Url] nvarchar(1000) NULL,
        [RelativePath] nvarchar(500) NULL,
        [OriginalFileName] nvarchar(300) NULL,
        [ContentType] nvarchar(150) NULL,
        [FileSizeBytes] bigint NULL,
        [SortOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_LessonResources] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LessonResources_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [MessageAttachments] (
        [Id] int NOT NULL IDENTITY,
        [MessageId] int NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [OriginalFileName] nvarchar(300) NOT NULL,
        [RelativePath] nvarchar(500) NOT NULL,
        [ContentType] nvarchar(150) NULL,
        [FileSizeBytes] bigint NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByUserId] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedByUserId] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedByUserId] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        [SchoolId] int NOT NULL,
        CONSTRAINT [PK_MessageAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MessageAttachments_Messages_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [Messages] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [MessageRecipients] (
        [Id] int NOT NULL IDENTITY,
        [MessageId] int NOT NULL,
        [RecipientUserId] nvarchar(450) NOT NULL,
        [ReadAt] datetime2 NULL,
        CONSTRAINT [PK_MessageRecipients] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MessageRecipients_Messages_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [Messages] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE TABLE [PaymentAllocations] (
        [Id] int NOT NULL IDENTITY,
        [PaymentId] int NOT NULL,
        [StudentFeeId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PaymentAllocations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentAllocations_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PaymentAllocations_StudentFees_StudentFeeId] FOREIGN KEY ([StudentFeeId]) REFERENCES [StudentFees] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AcademicStages_SchoolId_IsActive] ON [AcademicStages] ([SchoolId], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AnnouncementTargets_AnnouncementId] ON [AnnouncementTargets] ([AnnouncementId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AssignmentSubmissions_AssignmentId] ON [AssignmentSubmissions] ([AssignmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BehaviourRecords_BehaviourCategoryId] ON [BehaviourRecords] ([BehaviourCategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BookCopies_BookId] ON [BookCopies] ([BookId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassSchedules_ClassroomId] ON [ClassSchedules] ([ClassroomId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassSchedules_ClassSectionId_DayOfWeek_TeachingPeriodId] ON [ClassSchedules] ([ClassSectionId], [DayOfWeek], [TeachingPeriodId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassSchedules_SubjectId] ON [ClassSchedules] ([SubjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassSchedules_TeacherId_DayOfWeek_TeachingPeriodId] ON [ClassSchedules] ([TeacherId], [DayOfWeek], [TeachingPeriodId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassSchedules_TeachingPeriodId] ON [ClassSchedules] ([TeachingPeriodId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassSections_GradeLevelId] ON [ClassSections] ([GradeLevelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CourseUnits_SchoolId_SubjectId] ON [CourseUnits] ([SchoolId], [SubjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EmergencyContacts_StudentId] ON [EmergencyContacts] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Exams_ClassSectionId] ON [Exams] ([ClassSectionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Exams_ExamPeriodId] ON [Exams] ([ExamPeriodId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Exams_SchoolId_ExamDate] ON [Exams] ([SchoolId], [ExamDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Exams_TeacherId] ON [Exams] ([TeacherId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FeePlans_FeeTypeId] ON [FeePlans] ([FeeTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GradeItems_GradeCategoryId] ON [GradeItems] ([GradeCategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_GradeLevels_AcademicStageId] ON [GradeLevels] ([AcademicStageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LessonClassSections_ClassSectionId] ON [LessonClassSections] ([ClassSectionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LessonProgresses_LessonId] ON [LessonProgresses] ([LessonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LessonResources_LessonId] ON [LessonResources] ([LessonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Lessons_CourseUnitId] ON [Lessons] ([CourseUnitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Lessons_SchoolId_SubjectId_Status] ON [Lessons] ([SchoolId], [SubjectId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Lessons_SubjectId] ON [Lessons] ([SubjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Lessons_TeacherId] ON [Lessons] ([TeacherId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LibraryLoans_BookCopyId] ON [LibraryLoans] ([BookCopyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MessageAttachments_MessageId] ON [MessageAttachments] ([MessageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MessageRecipients_MessageId] ON [MessageRecipients] ([MessageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MessageRecipients_RecipientUserId] ON [MessageRecipients] ([RecipientUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_ParentMessageId] ON [Messages] ([ParentMessageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_SchoolId_StudentId_CreatedAt] ON [Messages] ([SchoolId], [StudentId], [CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_SenderUserId] ON [Messages] ([SenderUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_StudentId] ON [Messages] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_TeacherId] ON [Messages] ([TeacherId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PaymentAllocations_PaymentId] ON [PaymentAllocations] ([PaymentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PaymentAllocations_StudentFeeId] ON [PaymentAllocations] ([StudentFeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Permissions_Key] ON [Permissions] ([Key]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_QuestionOptions_QuestionId] ON [QuestionOptions] ([QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_QuizAttempts_QuizId] ON [QuizAttempts] ([QuizId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_QuizQuestions_QuestionId] ON [QuizQuestions] ([QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoutineLessons_GradeLevelId] ON [RoutineLessons] ([GradeLevelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoutineLessons_SchoolId_GradeLevelId_LessonName] ON [RoutineLessons] ([SchoolId], [GradeLevelId], [LessonName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SchoolBranches_SchoolId_NameAr] ON [SchoolBranches] ([SchoolId], [NameAr]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Schools_NameAr] ON [Schools] ([NameAr]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SchoolSettings_SchoolId] ON [SchoolSettings] ([SchoolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Semesters_AcademicYearId] ON [Semesters] ([AcademicYearId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentAddresses_StudentId] ON [StudentAddresses] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentAttendances_AttendanceSessionId_StudentId] ON [StudentAttendances] ([AttendanceSessionId], [StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentBadges_BadgeId] ON [StudentBadges] ([BadgeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentDocuments_StudentId] ON [StudentDocuments] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentEducationalProfiles_StudentId] ON [StudentEducationalProfiles] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentEnrollments_AcademicYearId] ON [StudentEnrollments] ([AcademicYearId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentEnrollments_ClassSectionId] ON [StudentEnrollments] ([ClassSectionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentEnrollments_GradeLevelId] ON [StudentEnrollments] ([GradeLevelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentEnrollments_SchoolId] ON [StudentEnrollments] ([SchoolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentEnrollments_StudentId_AcademicYearId] ON [StudentEnrollments] ([StudentId], [AcademicYearId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentFees_FeePlanId] ON [StudentFees] ([FeePlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentGrades_GradeItemId] ON [StudentGrades] ([GradeItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentGuardians_GuardianId] ON [StudentGuardians] ([GuardianId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StudentHealthProfiles_StudentId] ON [StudentHealthProfiles] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentHobbies_StudentId] ON [StudentHobbies] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentNotes_StudentId] ON [StudentNotes] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentQimamCertificates_SchoolId_CertificateDate] ON [StudentQimamCertificates] ([SchoolId], [CertificateDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentQimamCertificates_StudentId] ON [StudentQimamCertificates] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Students_PassportOrCardId] ON [Students] ([PassportOrCardId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Students_SchoolId] ON [Students] ([SchoolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Students_SchoolId_StudentNumber] ON [Students] ([SchoolId], [StudentNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Students_Status] ON [Students] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StudentTransportAssignments_TransportRouteId] ON [StudentTransportAssignments] ([TransportRouteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SubjectGradeAssignments_GradeLevelId] ON [SubjectGradeAssignments] ([GradeLevelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SubjectGradeAssignments_SubjectId] ON [SubjectGradeAssignments] ([SubjectId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Teachers_DocumentId] ON [Teachers] ([DocumentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Teachers_EmployeeId] ON [Teachers] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Teachers_SchoolId] ON [Teachers] ([SchoolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TransportRoutes_VehicleId] ON [TransportRoutes] ([VehicleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserPermissions_PermissionId] ON [UserPermissions] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserSchoolAssignments_SchoolBranchId] ON [UserSchoolAssignments] ([SchoolBranchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserSchoolAssignments_SchoolId] ON [UserSchoolAssignments] ([SchoolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserSchoolAssignments_UserId_SchoolId] ON [UserSchoolAssignments] ([UserId], [SchoolId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260809105916_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260809105916_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

