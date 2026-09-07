# Phase 1A — Roles and Permission Matrix

## 1. Roles

| # | Role key | Arabic label (UI) | Typical scope |
|---|----------|-------------------|---------------|
| 1 | `SuperAdministrator` | المشرف العام | All schools |
| 2 | `CentralAdministrator` | الإدارة المركزية | Assigned schools |
| 3 | `SchoolAdministrator` | مدير المدرسة | One school |
| 4 | `AcademicSupervisor` | المشرف الأكاديمي | School |
| 5 | `Teacher` | معلم | School / assigned classes |
| 6 | `Student` | طالب | Own record |
| 7 | `Parent` | ولي أمر | Linked children |
| 8 | `Accountant` | محاسب | School finance |
| 9 | `RegistrationOfficer` | مسؤول التسجيل | School admissions |
| 10 | `AttendanceOfficer` | مسؤول الحضور | School attendance |
| 11 | `GuidanceCounsellor` | المرشد التربوي | School guidance |
| 12 | `HealthOfficer` | المسؤول الصحي | School health |
| 13 | `Librarian` | أمين المكتبة | School library |
| 14 | `TransportationOfficer` | مسؤول النقل | School transport |
| 15 | `ContentManager` | مدير المحتوى | Announcements/content |
| 16 | `SupportUser` | مستخدم الدعم | Limited helpdesk |

Roles are Identity roles. Permissions are assigned to roles (and optionally directly to users). School/branch scope is enforced via `UserSchoolAssignment` / `UserSchoolRole`, not by role name alone.

## 2. Permission catalog (initial)

Permission keys use `Area.Action` naming.

### Platform

```text
Schools.View | Schools.Create | Schools.Edit | Schools.Delete
Branches.View | Branches.Create | Branches.Edit | Branches.Delete
SchoolSettings.View | SchoolSettings.Edit
SystemSettings.View | SystemSettings.Edit
Users.View | Users.Create | Users.Edit | Users.Disable | Users.ForcePasswordReset
Roles.View | Roles.Create | Roles.Edit | Roles.AssignPermissions
Permissions.View | Permissions.Assign
AuditLogs.View
LoginHistory.View
```

### Academic structure

```text
AcademicYears.View | AcademicYears.Create | AcademicYears.Edit | AcademicYears.Close | AcademicYears.Copy
Semesters.View | Semesters.Manage
Stages.View | Stages.Manage
Grades.View | Grades.Manage
Sections.View | Sections.Manage
Subjects.View | Subjects.Manage
Timetables.View | Timetables.Manage | Timetables.Publish | Timetables.Export
Enrollments.View | Enrollments.Create | Enrollments.Transfer | Enrollments.Promote | Enrollments.Withdraw
```

### People

```text
Students.View | Students.Create | Students.Edit | Students.Delete | Students.Export
Guardians.View | Guardians.Create | Guardians.Edit
StudentDocuments.View | StudentDocuments.Upload | StudentDocuments.Delete
StudentHealth.View | StudentHealth.Edit
StudentGuidance.View | StudentGuidance.Edit
Teachers.View | Teachers.Create | Teachers.Edit
Employees.View | Employees.Create | Employees.Edit
```

### LMS

```text
Lessons.View | Lessons.Create | Lessons.Edit | Lessons.Publish | Lessons.Delete
LessonProgress.View
Homework.View | Homework.Create | Homework.Edit | Homework.Publish | Homework.Review | Homework.Submit
QuestionBank.View | QuestionBank.Manage
Quizzes.View | Quizzes.Create | Quizzes.Publish | Quizzes.Attempt | Quizzes.Grade
```

### Academic operations

```text
Exams.View | Exams.Create | Exams.Edit | Exams.Publish | Exams.Schedule
Grades.View | Grades.Enter | Grades.Submit | Grades.Approve | Grades.Publish | Grades.Lock | Grades.Export
Attendance.View | Attendance.Record | Attendance.Edit | Attendance.Approve | Attendance.Reports
AttendanceExcuses.View | AttendanceExcuses.Create | AttendanceExcuses.Approve
```

### Communication & engagement

```text
Announcements.View | Announcements.Create | Announcements.Publish | Announcements.Delete
Events.View | Events.Manage | Events.Register
Notifications.View | Notifications.Send
Messages.View | Messages.Send
Meetings.View | Meetings.Manage | Meetings.Book
Behaviour.View | Behaviour.Create | Behaviour.Edit | Behaviour.ConfidentialView
Badges.View | Badges.Manage | Badges.Award
```

### Finance & support

```text
Payments.View | Payments.Receive | Payments.Cancel | Payments.Refund | Payments.Reports
Fees.View | Fees.Manage | Fees.Assign
Discounts.View | Discounts.Approve
Transport.View | Transport.Manage
Library.View | Library.Manage | Library.Loan
Reports.Academic | Reports.Attendance | Reports.Homework | Reports.Finance | Reports.System
```

## 3. Role × permission matrix (core)

Legend: **F** = full (view+manage within scope) · **V** = view · **O** = own/linked only · **—** = none · **S** = special elevated

| Capability area | Super | Central | School Admin | Acad. Sup. | Teacher | Student | Parent | Accountant | Reg. Off. | Att. Off. | Counsellor | Health | Librarian | Transport | Content | Support |
|-----------------|-------|---------|--------------|------------|---------|---------|--------|------------|-----------|-----------|------------|--------|-----------|-----------|---------|---------|
| Schools/Branches | F | V/Edit assigned | V own | V | — | — | — | — | — | — | — | — | — | — | — | V limited |
| Users & Roles | F | F assigned | F school | V | — | — | — | — | V/Create students parents | — | — | — | — | — | — | V |
| Academic structure | F | F | F | F | V assigned | V own | V child | — | V | V | V | — | — | — | — | — |
| Students | F | F | F | F | V class | O | O child | V financial | F | V | V | V health | V | V transport | — | V |
| Health profiles | S | S | S | — | — | O limited | O limited | — | — | — | V | F | — | — | — | — |
| Guidance confidential | S | S | S | V | — | — | — | — | — | — | F | — | — | — | — | — |
| Lessons / Homework | F | F | F | F | F class | O | V child | — | — | — | — | — | — | — | V | — |
| Quizzes / Exams | F | F | F | F | F class | O attempt | V child | — | — | — | — | — | — | — | — | — |
| Grades | F | F | F | Approve | Enter/Submit | O | O child | — | — | — | — | — | — | — | — | — |
| Attendance | F | F | F | F | Record class | O | O child | — | — | F | V | V | — | — | — | — |
| Announcements | F | F | F | Create | V | V | V | V | V | V | V | V | V | V | F | V |
| Messaging | F | F | F | F | Send class/parents | Limited | With school/teachers | — | — | — | With parents | — | — | — | — | Support queue |
| Fees/Payments | F | F | F | — | — | O | O child | F | V | — | — | — | — | V fees | — | — |
| Transport | F | F | F | — | — | O | O child | V | — | — | — | — | — | F | — | — |
| Library | F | F | F | — | — | O | V child | — | — | — | — | — | F | — | — | — |
| Audit logs | F | V | V school | — | — | — | — | — | — | — | — | — | — | — | — | V limited |
| Reports | F | F | F | Academic | Class | — | — | Finance | Reg | Attendance | Behaviour | Health | Library | Transport | Content | — |

Exact claim grants will be seeded in Phase 1B/3 and editable by Super/Central/School admins within their scope.

## 4. Scope rules

1. **School assignment required** for all non–Super Admin users before accessing school data.
2. **Branch restriction** optional; when set, section/classroom queries respect branch.
3. **Teacher scope** limited to `TeacherAssignment` classes/subjects unless elevated.
4. **Parent scope** limited to `StudentGuardian` links; active child stored in session/claim.
5. **Student scope** limited to own `StudentId`.
6. **Disable account** sets Identity lockout / custom `IsActive = false` without deleting history.
7. **Force password reset** flag blocks portal use until change.

## 5. Critical actions always audited

Login, failed login, user/role/permission changes, student create/update/transfer, attendance correction, grade create/modify/approve, payment create/cancel/refund, health record access, document download, data export, soft delete.
