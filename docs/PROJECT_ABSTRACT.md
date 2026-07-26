# Project Abstract — Online Learning Center (lms-mvp)

Use this document to resume work after a pause. It captures what the product is, where the code lives, what is already built, and the next implementation slices.

## One-line summary

**ASP.NET Web Forms + SQL Server LMS starter** for universities, training institutes, and corporate learning — role portals, secure exam-by-code flow, post-pass survey gate, and QR certificate verification — targeted at Windows Server / IIS.

## Product intent

Build a responsive Online Learning Center with:

| Role | Purpose |
|------|---------|
| **Student** | Courses, progress, assessments, certificates, payments |
| **Instructor** | Curriculum, live classes, grading, announcements |
| **Admin** | Users/roles, finance, exam governance, reports/KPIs |

Critical learning workflow to preserve:

1. Student enters a validated **exam code**
2. Timed exam session (default 60 minutes), autosave / auto-submit
3. Pass/fail evaluation
4. **Required survey after pass** before certificate generation
5. Certificate issuance with **QR verification**

## Repository state (as of 2026-07-26)

| Item | Status |
|------|--------|
| `main` | Nearly empty (`# lms-mvp` only) |
| Implementation branch | `cursor/online-learning-center-webforms-4f3b` |
| Open PR | [#1 — Scaffold Online Learning Center Web Forms starter](https://github.com/badour/lms-mvp/pull/1) (draft) |
| Prior agent | [Online learning center platform](https://cursor.com/agents/bc-4a996a61-a009-42e3-80b8-37b031e54f3b) |

**Important:** All scaffolded code, SQL, and docs currently live on the PR branch above, not on `main`. To resume implementation, check out that branch (or merge PR #1 first).

```bash
git fetch origin
git checkout cursor/online-learning-center-webforms-4f3b
```

## Stack

- **UI:** ASP.NET Web Forms (.NET Framework 4.8), Bootstrap 5.3, custom CSS/JS
- **Theme:** dark navy/blue + bright gold (`Content/site.css`)
- **Data:** SQL Server schema + reporting views
- **Host target:** Windows Server + IIS (+ SQL Server or Azure SQL)
- **Auth (configured, not implemented):** Forms Authentication in `Web.config`

## Layout on the implementation branch

```
README.md
docs/
  solution-proposal.md          # executive blueprint & modules
  windows-server-deployment.md  # IIS + SQL checklist
database/
  01_core_schema.sql            # full normalized schema
  02_reporting_views.sql        # enrollment / exam / revenue / cert views
src/
  OnlineLearningCenter.sln
  OnlineLearningCenter.Web/     # Web Forms site
    Site.Master, Default.aspx, About.aspx, Contact.aspx
    Student/   Dashboard, Courses, Assessments, Certificates
    Instructor/ Dashboard, Courses, LiveClasses
    Admin/      Dashboard, Users, Finance
    Exams/      Access
    Certificates/ Verify
    Content/site.css, Scripts/site.js
    Web.config, *.csproj
```

## What is done

- Solution + Web Forms project scaffolding (`.sln` / `.csproj` with WebApplication targets + output-path fixes)
- Public site shell and role portal **page shells** (static markup / sample metrics)
- Shared master page, nav, dark-blue / gold theme
- Client-side exam countdown demo (`Scripts/site.js`)
- SQL schema covering roles/users, students/instructors, courses/lessons/progress, live classes, assessments/exam codes/sessions/answers, surveys, certificates, payments, announcements
- Reporting views: enrollment stats, exam performance, revenue, certificate status
- Proposal + Windows/IIS deployment docs
- Forms auth mode declared; connection string placeholder present

## What is not done (gaps)

Pages are **UI placeholders only** — no code-behind business logic, no DAL/services, no live SQL binding.

Highest-value next work:

1. **Authentication & RBAC** — login/logout, password hashing, role gates for Admin / Instructor / Student / Finance / Exam Officer
2. **Data access layer** — parameterized ADO.NET or repository services over `DefaultConnection`
3. **Wire dashboards** — replace hard-coded cards with `vw_*` views and table queries
4. **Exam engine** — code validation, session create, timer server-side, autosave answers, auto-submit, scoring
5. **Survey gate + certificates** — enforce post-pass survey; PDF/image cert + QR verify endpoint
6. **Instructor course CRUD** — sections, lessons, resources, publish workflow
7. **Finance** — payments/invoices, optional payment gateway
8. **Live classes** — schedule + attendance; later conferencing API
9. **Hardening** — HTTPS, anti-forgery, audit logs, lockout, file upload storage

## Domain model (schema anchors)

Primary tables already defined in `database/01_core_schema.sql`:

`Roles`, `Users`, `Universities`, `Students`, `Instructors` →  
`CourseCategories`, `Courses`, `CourseSections`, `Lessons`, `LessonResources`, `Enrollments`, `StudentLessonProgress` →  
`ClassGroups`, `ClassStudents`, `LiveClassSessions`, `LiveClassAttendance` →  
`Assessments`, `AssessmentQuestions`, `AssessmentQuestionOptions`, `ExamCodes`, `ExamSessions`, `ExamAnswers` →  
`CertificateSurveys`, `CertificateSurveyQuestions`, `CertificateSurveyResponses`, `Certificates` →  
`SubscriptionPlans`, `Payments`, `Announcements`

Reporting views in `02_reporting_views.sql`:

- `vw_DashboardEnrollmentStats`
- `vw_DashboardExamPerformance`
- `vw_DashboardRevenue`
- `vw_CertificateStatusSummary`

## Local restart checklist (Windows)

1. Open `src/OnlineLearningCenter.sln` in Visual Studio 2022
2. Create DB `OnlineLearningCenterDb`
3. Run `database/01_core_schema.sql` then `02_reporting_views.sql`
4. Set connection string in `src/OnlineLearningCenter.Web/Web.config`
5. Run with IIS Express

> Linux/cloud agents can edit the scaffold and SQL, but **cannot fully build/run** classic Web Forms (.NET Framework / VS WebApplication targets). Validate builds on Windows.

## Suggested first implementation slice

When restarting coding, prefer this thin vertical slice over broad UI polish:

1. Seed roles + a demo admin/instructor/student in SQL
2. Implement Forms login against `Users.PasswordHash`
3. Protect `/Admin`, `/Instructor`, `/Student` by role
4. Bind `Student/Dashboard.aspx` metrics from `Enrollments` / exam / certificate queries
5. Implement `Exams/Access.aspx` → create `ExamSessions` row and enforce code + eligibility

## Design / product notes to keep

- Brand: **Online Learning Center**
- Visual: navy background, gold accents (already in `site.css`)
- Exam default duration: **60 minutes**
- Certificate path depends on **passed exam + completed survey**
- Hosting assumption remains **Windows Server + IIS + SQL Server** unless product direction changes

## Related docs on the implementation branch

- `docs/solution-proposal.md` — full module blueprint
- `docs/windows-server-deployment.md` — production IIS checklist
- Root `README.md` — quick start and suggested next build steps
