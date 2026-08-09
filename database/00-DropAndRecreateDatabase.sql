/*
  Use only when a previous schema apply failed partway.
  WARNING: destroys all data in SchoolLMS.
*/
IF DB_ID(N'SchoolLMS') IS NOT NULL
BEGIN
    ALTER DATABASE [SchoolLMS] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [SchoolLMS];
END
GO

CREATE DATABASE [SchoolLMS];
GO

ALTER DATABASE [SchoolLMS] SET RECOVERY SIMPLE;
GO
