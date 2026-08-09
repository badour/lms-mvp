# School LMS — SQL Server schema

Use these scripts to run the system against **SQL Server** instead of SQLite.

## Files

| File | Purpose |
|------|---------|
| `00-CreateDatabase.sql` | Creates empty database `SchoolLMS` |
| `SchoolLMS.Schema.sql` | Idempotent EF Core schema (tables, indexes, FKs, `__EFMigrationsHistory`) |

Schema source of truth: EF migration  
`src/SchoolLMS.Infrastructure/Persistence/Migrations/20260809101118_InitialCreate.cs`

## Option A — run SQL scripts (SSMS / Azure Data Studio / sqlcmd)

```bash
# 1) Create database
sqlcmd -S . -E -i database/00-CreateDatabase.sql

# 2) Apply schema
sqlcmd -S . -E -d SchoolLMS -i database/SchoolLMS.Schema.sql
```

## Option B — EF migrate (recommended for app hosts)

```bash
dotnet ef database update \
  --project src/SchoolLMS.Infrastructure \
  --startup-project src/SchoolLMS.Web
```

With SQL Server config active (see below), app startup also calls `MigrateAsync()` then seeds demo users/data.

## Point the app at SQL Server

Copy/merge `src/SchoolLMS.Web/appsettings.SqlServer.json`, or set:

```json
{
  "Database": { "Provider": "SqlServer" },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SchoolLMS;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

Examples:

```bash
# Web
dotnet run --project src/SchoolLMS.Web --environment SqlServer

# Or set env vars
export Database__Provider=SqlServer
export ConnectionStrings__DefaultConnection='Server=.;Database=SchoolLMS;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true'
dotnet run --project src/SchoolLMS.Web
```

## After schema is applied

Start the web app once so `DatabaseSeeder` creates roles, permissions, admin user, and demo schools.

Default admin: `admin` / `Admin@12345`

## Regenerate schema after model changes

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet ef migrations add <Name> \
  --project src/SchoolLMS.Infrastructure \
  --startup-project src/SchoolLMS.Web \
  --output-dir Persistence/Migrations

dotnet ef migrations script --idempotent \
  --project src/SchoolLMS.Infrastructure \
  --startup-project src/SchoolLMS.Web \
  --output database/SchoolLMS.Schema.sql
```
