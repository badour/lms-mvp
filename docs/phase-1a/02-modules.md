# Phase 1A — Complete Module List

Modules are bounded by business capability. Each module owns its Application services, Domain entities, Infrastructure configurations, and Web/API surface slices.

## Platform & tenancy

| Code | Module | Description |
|------|--------|-------------|
| M01 | Schools & Branches | Multi-school registry, branches, logos, contacts |
| M02 | School Settings | Theme, working days, attendance rules, grade system, SMS/notification settings |
| M03 | Identity & Access | Users, roles, permissions, school/branch assignments, login history |
| M04 | Audit & Security | Audit logs, failed logins, device/IP tracking, lockout, CAPTCHA hooks |
| M05 | File Management | Central file metadata, storage providers, download tracking |
| M06 | Background Jobs | Hangfire jobs, retries, failure logging |
| M07 | System Settings | Global platform settings, feature flags |

## Academic structure

| Code | Module | Description |
|------|--------|-------------|
| M08 | Academic Years & Semesters | Open/close years, copy settings, freeze records |
| M09 | Stages, Grades, Sections | Hierarchy and section capacity |
| M10 | Subjects & Assignments | Subjects, grade mapping, teacher assignments |
| M11 | Classrooms & Periods | Rooms, teaching/break periods |
| M12 | Timetable | Weekly/daily schedules, conflict detection, print/PDF |
| M13 | Student Enrollment | Enroll, promote, repeat, transfer, withdraw |

## People

| Code | Module | Description |
|------|--------|-------------|
| M14 | Students | Full student profile (personal, academic, address, docs) |
| M15 | Guardians | Multi-guardian links, primary/financial flags, child switcher |
| M16 | Emergency Contacts | Prioritized contacts |
| M17 | Health Profiles | Restricted medical data |
| M18 | Educational & Behavioural Profiles | SEN, strengths/weaknesses, counsellor notes |
| M19 | Talents & Activities | Clubs, sports, achievements evidence |
| M20 | Employees & Teachers | Staff records linked to Identity users |

## LMS (e-learning)

| Code | Module | Description |
|------|--------|-------------|
| M21 | Course Units & Lessons | Units, lessons, publish lifecycle |
| M22 | Lesson Resources | Videos, files, links, activities |
| M23 | Lesson Progress | Open/watch/complete tracking |
| M24 | Homework & Assignments | Create, submit, review, late rules |
| M25 | Question Bank | Hierarchy by subject/unit/topic/difficulty |
| M26 | Online Quizzes | Attempts, auto-grade, manual grade essays |

## Academic operations

| Code | Module | Description |
|------|--------|-------------|
| M27 | School Exams | Offline exam types, schedules, invigilators |
| M28 | Grades & Results | Categories, workflows, transcripts, analytics |
| M29 | Attendance | Daily/period attendance, excuses, corrections, alerts |

## Communication & engagement

| Code | Module | Description |
|------|--------|-------------|
| M30 | Announcements | Targeted, prioritized, acknowledgement |
| M31 | Events | School events, registration, capacity |
| M32 | Notifications | In-app, email, SMS, push, WhatsApp provider hooks |
| M33 | Messaging | Secure internal messaging + read receipts |
| M34 | Parent Meetings | Slots, booking, approval, reminders |
| M35 | Behaviour & Guidance | Behaviour records, counselling, interventions |
| M36 | Badges & Achievements | Badge catalog and awards |

## Finance & supporting

| Code | Module | Description |
|------|--------|-------------|
| M37 | Fees & Payments | Fee plans, installments, receipts, refunds (`decimal`) |
| M38 | Transportation | Vehicles, routes, stops, student assignments |
| M39 | Library | Catalog, loans, reservations, penalties |
| M40 | Reports & Analytics | Academic, attendance, homework, finance, system reports |

## Delivery surfaces

| Code | Module | Description |
|------|--------|-------------|
| M41 | Web Portals | Public + Student + Parent + Teacher + Admin UI |
| M42 | Mobile API | JWT/refresh, versioned REST, Swagger |
| M43 | Localization | Arabic/English resources, date formats, Iraqi terminology |
| M44 | Dashboards | Role-specific dashboards fed by services (no hard-coded stats) |

## Module → primary roles (summary)

| Module group | Primary consumers |
|--------------|-------------------|
| Platform/tenancy | Super Admin, Central Admin |
| Academic structure | School Admin, Academic Supervisor |
| People | Registration Officer, School Admin |
| LMS | Teacher, Student, Parent (view) |
| Academic ops | Teacher, Attendance Officer, Academic Supervisor |
| Communication | Content Manager, Teachers, Admins |
| Finance | Accountant |
| Transport | Transportation Officer |
| Library | Librarian |
| Guidance | Guidance Counsellor, Health Officer |
| Reports | Admins, Supervisors, Accountant |
