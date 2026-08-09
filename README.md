# School LMS

Multi-School Management and Learning Management System (SIS + LMS) for private schools.

**Stack:** ASP.NET Core 8 MVC · EF Core · SQL Server/SQLite · Identity · JWT API · Razor · Bootstrap 5 RTL · Arabic-first

## Solution

```text
SchoolLMS.sln
src/
  SchoolLMS.Domain
  SchoolLMS.Application
  SchoolLMS.Infrastructure
  SchoolLMS.Web
  SchoolLMS.Api
tests/
  SchoolLMS.Tests
docs/
  phase-1a/          # architecture package
  deployment/        # IIS guide
```

## Quick start (development)

Requirements: **.NET 8 SDK** (and for Visual Studio: VS 2022 17.8+ with the **ASP.NET and web development** workload)

### Visual Studio

1. Open **`SchoolLMS.sln`** (not a single `.csproj`).
2. Right-click the solution → **Restore NuGet Packages**.
3. Set **`SchoolLMS.Web`** as the Startup Project.
4. Press F5 / Run.

If you see `NU1105` about `SchoolLMS.Web.csproj`, close Visual Studio, delete all `bin` / `obj` folders, reopen **`SchoolLMS.sln`**, then restore again:

```powershell
dotnet restore SchoolLMS.sln
dotnet build SchoolLMS.sln
```

### Command line

```bash
dotnet restore SchoolLMS.sln
dotnet build SchoolLMS.sln
dotnet test SchoolLMS.sln
dotnet run --project src/SchoolLMS.Web
```

Open the site URL from the console (typically `https://localhost:7xxx` or `http://localhost:5xxx`).

Default database provider is **SQLite** for local/dev.

### SQL Server

Schema scripts live in [`database/`](database/README.md):

```bash
sqlcmd -S . -E -i database/00-CreateDatabase.sql
sqlcmd -S . -E -d SchoolLMS -i database/SchoolLMS.Schema.sql
```

Or apply EF migrations:

```bash
dotnet ef database update --project src/SchoolLMS.Infrastructure --startup-project src/SchoolLMS.Web
```

Switch the app to SQL Server (see also `src/SchoolLMS.Web/appsettings.SqlServer.json`):

```json
"Database": { "Provider": "SqlServer" },
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=SchoolLMS;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

```bash
dotnet run --project src/SchoolLMS.Web --environment SqlServer
```

### Demo accounts (seeded)

| Role | Username | Password |
|------|----------|----------|
| Super Admin | `admin` | `Admin@12345` |
| School Admin | `schooladmin1` | `SchoolAdmin@12345` |
| Teacher | `teacher1` | `Teacher@12345` |
| Student | `student1` | `Student@12345` |
| Parent | `parent1` | `Parent@12345` |

## What is implemented

- Clean layered architecture + multi-school `SchoolId` tenancy
- ASP.NET Core Identity with 16 roles and permission catalog
- Audit logging + login history
- Seeded Arabic demo data (4 schools, academic structure, demo users)
- Admin portal: dashboard (Chart.js), schools CRUD, students list/create
- Student / Teacher / Parent dashboards (service-driven, not hard-coded)
- Mobile API: `/api/v1/auth/login|refresh|logout`, `/api/v1/students/me/dashboard`
- Hangfire dashboard at `/hangfire`
- Arabic RTL layout, culture switch ar/en
- Unit + integration tests

## Design docs

See [docs/phase-1a/00-overview.md](docs/phase-1a/00-overview.md) for full module list, ERD, roles, and phased plan.

## Next implementation slices

- Attendance entry UI + parent absence notifications
- Lessons / homework / quiz full workflows
- Grade approval workflow
- Fees/payments cashier screens
- Expand API surface for Flutter/MAUI
- EF migrations checked in for SQL Server production

## Deployment

See [docs/deployment/iis-deployment.md](docs/deployment/iis-deployment.md).
