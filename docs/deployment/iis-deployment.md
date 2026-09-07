# IIS Deployment Guide (Windows Server)

## Prerequisites

- Windows Server 2019+
- IIS with ASP.NET Core Hosting Bundle (.NET 8)
- SQL Server 2019+ (local or remote)
- HTTPS certificate

## Publish

```bash
dotnet publish src/SchoolLMS.Web/SchoolLMS.Web.csproj -c Release -o C:\publish\SchoolLMS.Web
dotnet publish src/SchoolLMS.Api/SchoolLMS.Api.csproj -c Release -o C:\publish\SchoolLMS.Api
```

## Configuration

Set in IIS environment variables or `appsettings.Production.json`:

```json
{
  "Database": { "Provider": "SqlServer" },
  "ConnectionStrings": {
    "DefaultConnection": "Server=SQLHOST;Database=SchoolLMS;User Id=...;Password=...;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<long-random-secret>",
    "Issuer": "SchoolLMS",
    "Audience": "SchoolLMS"
  }
}
```

## Database

**Option 1 — SQL scripts** (see [`database/README.md`](../../database/README.md)):

```bash
sqlcmd -S SQLHOST -E -i database/00-CreateDatabase.sql
sqlcmd -S SQLHOST -E -d SchoolLMS -i database/SchoolLMS.Schema.sql
```

**Option 2 — EF migrate:**

```bash
dotnet ef database update --project src/SchoolLMS.Infrastructure --startup-project src/SchoolLMS.Web
```

Or let the application migrate + seed on startup in controlled environments (`MigrateAsync` when `Database:Provider=SqlServer`).

## IIS sites

1. Create app pool: No Managed Code, Integrated
2. Bind Web site to `SchoolLMS.Web` publish folder
3. Bind API site (optional separate host) to `SchoolLMS.Api`
4. Enable HTTPS
5. Grant app-pool identity write access to `App_Data`

## First login

Seeded super admin (change immediately):

- Username: `admin`
- Password: `Admin@12345`
