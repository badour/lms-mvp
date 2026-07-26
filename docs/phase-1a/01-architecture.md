# Phase 1A — Solution Architecture

## 1. Architectural style

Clean layered architecture with clear dependency direction:

```text
SchoolLMS.Web  ──┐
                 ├──► SchoolLMS.Application ──► SchoolLMS.Domain
SchoolLMS.Api  ──┘              ▲
                                │
                     SchoolLMS.Infrastructure
```

| Project | Responsibility |
|---------|----------------|
| **SchoolLMS.Domain** | Entities, enums, value objects, domain interfaces, domain rules. No infrastructure or UI. |
| **SchoolLMS.Application** | Use cases, DTOs, validators, mapping, service interfaces, authorization policies helpers. |
| **SchoolLMS.Infrastructure** | EF Core, Identity, repositories, file/email/SMS providers, Hangfire, Serilog sinks, migrations. |
| **SchoolLMS.Web** | MVC controllers, Razor views, portal layouts (student/parent/teacher/admin), localization, anti-forgery. |
| **SchoolLMS.Api** | Versioned REST API (`/api/v1`), JWT auth, Swagger, mobile-facing contracts. |
| **SchoolLMS.Tests** | Unit, integration, permission, service, and API tests. |

**Rule:** Controllers and API endpoints call Application services only. Views never contain SQL or business rules. Entities are not exposed directly to clients.

## 2. Multi-school tenancy

One SQL Server database. Soft multi-tenancy by `SchoolId`.

```text
Platform (Super Admin)
 └── Schools
      └── Branches
           └── Academic structure, people, LMS, finance, ...
```

| Actor | Scope |
|-------|-------|
| Super Administrator | All schools |
| Central Administrator | Assigned schools (one, several, or all) |
| School Administrator | Assigned school (+ optional branch limits) |
| Staff roles | Assigned school(s)/branch(es) via `UserSchoolAssignment` |
| Student / Parent | Own school and linked student records only |

**Enforcement points**

1. Application services receive `ICurrentUserContext` (`UserId`, roles, school IDs, branch IDs, permissions).
2. Queries for tenant data always filter by authorized `SchoolId` set.
3. Super Admin may omit school filter when intentionally global.
4. API never trusts client-supplied `studentId` / `schoolId` without membership checks.
5. Global query filters in EF Core for `IsDeleted` and optional school scope where safe.

## 3. Cross-cutting foundations

| Concern | Approach |
|---------|----------|
| Identity | ASP.NET Core Identity (`ApplicationUser` : `IdentityUser`) |
| Authorization | Role claims + permission claims + school/branch claims |
| Validation | FluentValidation in Application layer |
| Mapping | AutoMapper profiles in Application |
| Logging | Serilog (file + console; optional Seq/SQL sink later) |
| Audit | `IAuditService` + `AuditLogs` table for critical actions |
| Soft delete | `ISoftDelete` on important business entities |
| Concurrency | SQL Server `rowversion` (`byte[] RowVersion`) |
| Localization | Resource files (`.ar`, `.en`), RTL layout default |
| Files | `IFileStorage` abstraction (local now, Azure Blob later) |
| Jobs | Hangfire for notifications, reminders, cleanup |
| API docs | Swagger/OpenAPI on `SchoolLMS.Api` |
| Errors | Global exception middleware + ProblemDetails (API) + localized error pages (Web) |

## 4. Base entity pattern

All important business tables inherit shared audit fields:

```csharp
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedByUserId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public abstract class SchoolOwnedEntity : AuditableEntity
{
    public int SchoolId { get; set; }
}
```

## 5. Portal surfaces

| Surface | Host | Users |
|---------|------|-------|
| Public | Web | Login, password reset, privacy, support |
| Student portal | Web + API | Students |
| Parent portal | Web + API | Guardians (child switcher) |
| Teacher portal | Web + API | Teachers |
| Administration | Web | School / central / super admins and officers |
| Mobile API | Api | Future Flutter / .NET MAUI clients |

Shared RTL layout: collapsible sidebar, top bar, breadcrumbs, toasts (SweetAlert2), DataTables, Chart.js.

## 6. Environments and deployment

| Environment | Purpose |
|-------------|---------|
| Development | Local IIS Express / Kestrel + local SQL |
| Staging | IIS + staging SQL, production-like config |
| Production | Windows Server IIS + SQL Server, HTTPS |

Configuration via `appsettings.{Environment}.json` + User Secrets / environment variables. No secrets in source control.

## 7. Security baseline

- HTTPS, secure cookies, anti-forgery on MVC forms
- Parameterized EF queries only
- Permission checks on every protected action
- Tenant `SchoolId` checks on every tenant query
- File type/size validation, random stored names
- Rate limiting / lockout / CAPTCHA after repeated failures
- Sensitive health/counselling data behind elevated permissions
- Audit for create/update/delete of critical records and sensitive reads

## 8. Performance baseline (≈3,000 users)

- Async EF Core, `AsNoTracking` for reads
- Server-side pagination (DataTables / API)
- Indexes on tenant and filter columns
- Cache stable lookups (stages, grade levels, fee types)
- Background jobs for heavy reports and notification fan-out
- Lazy loading disabled by default
