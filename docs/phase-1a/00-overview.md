# Phase 1A — Overview

## Goal

Define the architecture, modules, security model, data model, folder layout, dependencies, and delivery phases for the **School LMS** before any application code is written.

## Product summary

A production-oriented **multi-school SIS + LMS** for private schools in Iraq and the wider Arabic-speaking market.

| Concern | Decision |
|---------|----------|
| Tenancy | Single database, `SchoolId` isolation |
| UI language | Arabic RTL primary, optional English |
| Web | ASP.NET Core 8 MVC + Razor |
| API | ASP.NET Core 8 Web API (JWT) for future mobile |
| Data | EF Core Code First → SQL Server |
| AuthZ | Identity roles + fine-grained permissions + school/branch scope |
| Hosting | Windows Server + IIS + SQL Server |

Functional and visual inspiration may reference public school portals (for example epmdo-schools.org/schoolsys). **Do not copy** branding, images, proprietary content, or source code.

## Phase 1A deliverables

1. [Solution architecture](01-architecture.md)
2. [Complete module list](02-modules.md)
3. [Role and permission matrix](03-roles-permissions.md)
4. [Database ERD](04-database-erd.md)
5. [Project folder structure](05-folder-structure.md)
6. [Module dependencies](06-module-dependencies.md)
7. [Development phases](07-development-phases.md)

## Approval gate

**Stop here.** No implementation code should be generated until Phase 1A is approved.

Please confirm or request changes to:

- Project names and layer boundaries
- Multi-tenant rules (`SchoolId` / branch scope)
- Role and permission naming
- Domain boundaries and ERD coverage
- Phase sequencing

After approval, continue with **Phase 1B**: solution creation, base domain entities, Identity, audit, error handling, and multi-school foundation.
