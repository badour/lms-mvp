# Phase 1A — Development Phases

Incremental delivery. Each phase ends with compile verification and a short test checklist before the next phase starts.

## Phase map

| Phase | Name | Outcome |
|-------|------|---------|
| **1A** | Architecture & design | Docs only (this package) — **awaiting approval** |
| **1B** | Foundation scaffolding | Solution, DI, Identity, multi-school base, audit, errors, Serilog, localization shell |
| **2** | Database | Full entities, EF configs, DbContext, migrations, seed Arabic demo data, SQL script |
| **3** | Administration | Schools, years, grades/sections, students, guardians, teachers, roles/permissions UI |
| **4** | LMS | Units, lessons, resources, homework, submissions, question bank, quizzes |
| **5** | Academic operations | Attendance, exam schedules, grade entry/approval/publish, timetables |
| **6** | Communication | Announcements, notifications, messaging, meetings, badges |
| **7** | Finance & support | Fees, payments, discounts, transport, library, reports |
| **8** | API & deployment | Mobile JWT API, Swagger, IIS guide, backup plan, security checklist |

---

## Phase 1B — Foundation (after 1A approval)

**Create**

- `SchoolLMS.sln` and all projects with references
- `AuditableEntity`, `SchoolOwnedEntity`
- `ApplicationUser`, permissions entities, school/branch entities
- `ApplicationDbContext` (minimal), Identity wiring
- `ICurrentUserContext`, permission authorization handler
- Serilog, global exception middleware, anti-forgery, HTTPS defaults
- RTL `_Layout`, login page shell, culture switching (ar/en)
- Hangfire host registration (no business jobs yet)
- Initial migration + Super Admin seed

**Test**

- Solution builds
- Login page loads RTL Arabic
- Super Admin can sign in against seeded user
- Permission attribute blocks anonymous access

---

## Phase 2 — Database

**Create**

- Complete entity set from ERD
- Fluent configurations, indexes, unique constraints
- Migrations
- Seed: 4 schools, academic year, stages/grades/sections, roles/permissions matrix, demo users
- Generated SQL script export

**Test**

- `dotnet ef database update`
- Seed completes idempotently
- Unique constraints reject duplicates

---

## Phase 3 — Administration

**Create**

- CRUD for schools, branches, settings
- Academic years/semesters, grades, sections, subjects
- Student + guardian management with documents upload
- Teacher/employee management
- Roles/permissions assignment UI
- School-scoped authorization checks

**Test**

- School Admin cannot see other schools’ students
- Registration Officer can create student + guardian
- Permission matrix enforced on controller actions

---

## Phase 4 — LMS

**Create**

- Course units, lessons, resources
- Lesson progress tracking
- Homework create/submit/review
- Question bank + online quizzes (auto + manual grading)

**Test**

- Teacher publishes lesson to section
- Student submits homework before/after due date statuses
- Objective quiz auto-scores

---

## Phase 5 — Academic operations

**Create**

- Timetable with conflict warnings
- Attendance sessions + parent absence notification hook
- Exam periods/schedules
- Grade workflow Draft → … → Locked

**Test**

- Conflict detection blocks overlapping teacher periods
- Grade approval required before parent visibility
- Attendance unique per session/student

---

## Phase 6 — Communication

**Create**

- Announcements/events targeting
- Notification templates (ar/en) + Hangfire dispatch
- Messaging with read receipts
- Parent meeting booking
- Badges awards

**Test**

- Targeted announcement visible only to intended roles/sections
- Notification marked read
- Parent books available slot only

---

## Phase 7 — Finance & supporting modules

**Create**

- Fee plans, student fees, installments
- Payments/allocations/receipts/refunds (`decimal` only)
- Transport assignments
- Library loans
- Report queries + PDF/Excel export hooks

**Test**

- Partial payment updates remaining balance correctly
- Refund requires approval permission
- Reports filter by school/date

---

## Phase 8 — API & deployment

**Create**

- `/api/v1` auth + student/parent/teacher endpoints
- JWT + refresh tokens
- Swagger
- IIS deployment guide, backup/restore docs, production security checklist

**Test**

- API auth and refresh flow
- Parent cannot access unlinked `studentId`
- Smoke deploy notes validated on Windows checklist

---

## Agent working rules (from master prompt)

For every implementation phase:

1. List files to create/update with relative paths  
2. Provide complete file content (no “implementation goes here”)  
3. Explain migrations and how to test  
4. Include sample data and authorization/validation tests where relevant  
5. Verify compilation before moving on  
6. Do not generate the entire application in one step  

## Approval request

Please approve **Phase 1A** (or list requested changes).  
On approval, development continues with **Phase 1B — Foundation scaffolding**.
