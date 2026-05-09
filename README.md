# Online Learning Center - ASP.NET Web Forms Starter

This repository contains a starter implementation for a **responsive Online Learning Center** using:

- ASP.NET Web Forms (.NET Framework 4.8 style project)
- SQL Server database scripts
- Bootstrap + custom dark-blue / bright-gold theme
- Windows Server + IIS hosting guidance

## Included Deliverables

- `docs/solution-proposal.md` - executive summary, module architecture, and implementation blueprint.
- `docs/windows-server-deployment.md` - deployment checklist for IIS + SQL Server.
- `database/01_core_schema.sql` - normalized SQL Server schema for users, learning, exams, surveys, certificates, and finance.
- `database/02_reporting_views.sql` - starter analytical SQL views.
- `src/OnlineLearningCenter.Web` - Web Forms UI starter with Public, Student, Instructor, Admin, Exam, and Certificate pages.

## Quick Start (Development)

1. Open `src/OnlineLearningCenter.sln` in Visual Studio 2022 (Windows).
2. Create a SQL Server database (example: `OnlineLearningCenterDb`).
3. Run scripts in order:
   - `database/01_core_schema.sql`
   - `database/02_reporting_views.sql`
4. Update connection string in `src/OnlineLearningCenter.Web/Web.config`.
5. Run the site with IIS Express.

## Theme

- Background: dark navy/blue shades
- Accent: bright gold
- Style class definitions are in `Content/site.css`.

## Suggested Next Build Steps

- Add authentication (ASP.NET Identity or custom membership integration).
- Implement role-based authorization for Admin, Instructor, Student, Finance, Exam Officer.
- Replace static cards with live SQL-backed metrics and charts.
- Integrate payment gateway and conferencing APIs.
- Add PDF certificate rendering and QR verification endpoint.
