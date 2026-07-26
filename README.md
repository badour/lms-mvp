# School LMS

Multi-School Management and Learning Management System for a group of private schools.

**Stack:** ASP.NET Core 8 MVC · EF Core · SQL Server · Identity · REST API · Razor · Bootstrap 5 · Arabic RTL first

## Current status

**Phase 1A — Architecture and design (awaiting approval)**

Implementation code has not started. Review the Phase 1A package, then approve before Phase 1B / coding begins.

| Document | Description |
|----------|-------------|
| [docs/phase-1a/00-overview.md](docs/phase-1a/00-overview.md) | Executive overview and approval gate |
| [docs/phase-1a/01-architecture.md](docs/phase-1a/01-architecture.md) | Solution architecture |
| [docs/phase-1a/02-modules.md](docs/phase-1a/02-modules.md) | Complete module list |
| [docs/phase-1a/03-roles-permissions.md](docs/phase-1a/03-roles-permissions.md) | Roles and permission matrix |
| [docs/phase-1a/04-database-erd.md](docs/phase-1a/04-database-erd.md) | Database ERD (Mermaid) |
| [docs/phase-1a/05-folder-structure.md](docs/phase-1a/05-folder-structure.md) | Project folder structure |
| [docs/phase-1a/06-module-dependencies.md](docs/phase-1a/06-module-dependencies.md) | Module dependency graph |
| [docs/phase-1a/07-development-phases.md](docs/phase-1a/07-development-phases.md) | Incremental delivery plan |

## Scale target

- ~4 schools initially (multi-tenant by `SchoolId`)
- ~3,000 users
- Roles: students, parents, teachers, school admins, central admins, platform admins, supervisors, accountants, counsellors, and supporting staff

## Next step

Approve Phase 1A, then proceed to **Phase 1B**: solution scaffolding, base entities, Identity, multi-school infrastructure, and initial configuration.
