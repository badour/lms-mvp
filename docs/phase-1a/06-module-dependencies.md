# Phase 1A — Module Dependencies

## 1. Dependency graph (build order)

Lower layers must exist before upper modules can be implemented safely.

```mermaid
flowchart TB
    M03[M03 Identity and Access]
    M01[M01 Schools and Branches]
    M02[M02 School Settings]
    M04[M04 Audit and Security]
    M05[M05 File Management]
    M06[M06 Background Jobs]
    M07[M07 System Settings]

    M08[M08 Academic Years]
    M09[M09 Stages Grades Sections]
    M10[M10 Subjects]
    M11[M11 Classrooms Periods]
    M12[M12 Timetable]
    M13[M13 Enrollments]

    M14[M14 Students]
    M15[M15 Guardians]
    M20[M20 Employees Teachers]

    M21[M21 Lessons]
    M24[M24 Homework]
    M26[M26 Quizzes]
    M27[M27 School Exams]
    M28[M28 Grades]
    M29[M29 Attendance]

    M30[M30 Announcements]
    M32[M32 Notifications]
    M33[M33 Messaging]
    M35[M35 Behaviour]
    M36[M36 Badges]

    M37[M37 Fees Payments]
    M38[M38 Transport]
    M39[M39 Library]
    M40[M40 Reports]
    M41[M41 Web Portals]
    M42[M42 Mobile API]
    M44[M44 Dashboards]

    M03 --> M01
    M01 --> M02
    M03 --> M04
    M03 --> M05
    M03 --> M06
    M03 --> M07

    M01 --> M08
    M08 --> M09
    M09 --> M10
    M09 --> M11
    M10 --> M12
    M11 --> M12

    M01 --> M14
    M03 --> M14
    M14 --> M15
    M05 --> M14
    M09 --> M13
    M14 --> M13
    M03 --> M20
    M10 --> M20

    M10 --> M21
    M20 --> M21
    M21 --> M24
    M21 --> M26
    M10 --> M27
    M13 --> M28
    M27 --> M28
    M24 --> M28
    M13 --> M29
    M11 --> M29

    M01 --> M30
    M06 --> M32
    M03 --> M33
    M14 --> M35
    M14 --> M36

    M14 --> M37
    M13 --> M37
    M14 --> M38
    M14 --> M39

    M28 --> M40
    M29 --> M40
    M24 --> M40
    M37 --> M40

    M44 --> M41
    M44 --> M42
    M40 --> M44
    M32 --> M41
    M32 --> M42
```

## 2. Hard dependencies (cannot skip)

| Module | Depends on |
|--------|------------|
| Any school-owned module | M01 Schools, M03 Identity |
| Enrollments | Students, Grades/Sections, Academic Years |
| Timetable | Sections, Subjects, Teachers, Periods, Classrooms |
| Lessons / Homework / Quizzes | Subjects, Teachers, Enrollments (for targeting) |
| Grades | Enrollments, Subjects; optionally Exams/Homework/Quizzes as sources |
| Attendance | Sections, Students, Periods (for period mode) |
| Payments | Students, Fee plans; Enrollment often used for grade-based fee assignment |
| Parent portal child switcher | Guardians ↔ Students links |
| Notifications | Identity users + templates + jobs |
| Reports | Underlying transactional modules |
| Dashboards | Read models/services from many modules |

## 3. Soft / provider dependencies

| Capability | Abstraction | Initial provider | Later |
|------------|-------------|------------------|-------|
| Files | `IFileStorage` | Local disk | Azure Blob |
| Email | `IEmailSender` | SMTP / stub | Production SMTP |
| SMS | `ISmsSender` | Stub / configurable | Local Iraqi gateway |
| Push | `IPushSender` | Stub | Firebase |
| WhatsApp | `IWhatsAppSender` | Stub | Authorized API |
| Online payments | `IPaymentGateway` | Manual cashier first | Card gateway |
| CAPTCHA | `ICaptchaService` | Dev bypass | Production provider |
| Malware scan | `IFileScanner` | No-op hook | AV integration |

## 4. Circular dependency prevention

- Domain entities reference each other only through explicit FKs; no service calls in Domain.
- Application services may call other Application services carefully; prefer domain events / orchestration services for cross-module workflows (example: absence → notify parents).
- Web/Api never reference each other.
- Reports read via dedicated query services; they do not write operational data.

## 5. Suggested vertical-slice priority inside each phase

1. Entities + EF config + migration  
2. Seed / sample Arabic data  
3. Application service + validators  
4. Authorization checks  
5. Web UI pages  
6. API endpoints (where in scope)  
7. Tests (permission + service)  
8. Compile verification
