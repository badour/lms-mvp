/*
  School LMS — create empty SQL Server database
  Run this first in SSMS / sqlcmd against the target SQL Server instance.
*/
IF DB_ID(N'SchoolLMS') IS NULL
BEGIN
    CREATE DATABASE [SchoolLMS];
END
GO

ALTER DATABASE [SchoolLMS] SET RECOVERY SIMPLE;
GO
