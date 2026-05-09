# Windows Server Deployment Guide (IIS + SQL Server)

## 1. Prerequisites

- Windows Server 2019+ with IIS installed
- .NET Framework 4.8 Hosting Bundle
- SQL Server 2019+ (or Azure SQL)
- DNS + SSL certificate for HTTPS

## 2. Database Setup

1. Create database `OnlineLearningCenterDb`.
2. Execute:
   - `database/01_core_schema.sql`
   - `database/02_reporting_views.sql`
3. Create dedicated SQL login with least privilege:
   - read/write on application tables
   - execute on stored procedures (when added)

## 3. IIS Configuration

1. Create new App Pool:
   - .NET CLR v4.0
   - Integrated pipeline
   - `AlwaysRunning` (optional for performance)
2. Create website and bind host + SSL cert (443).
3. Point physical path to published output.
4. Grant folder permissions to IIS App Pool identity.

## 4. Application Configuration

- Set production connection string in `Web.config`.
- Enable custom errors and structured logging.
- Configure max request size for file uploads.

## 5. Security Hardening

- Enforce HTTPS redirect.
- Disable directory browsing.
- Set HTTP security headers.
- Restrict SQL port access by firewall rules.

## 6. Monitoring & Operations

- Enable IIS logs and Windows Event Viewer alerts.
- Run daily DB backups and periodic restore drills.
- Monitor CPU/memory/app pool recycles.
- Archive generated certificate artifacts and uploaded student files.
