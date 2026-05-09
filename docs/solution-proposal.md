# Proposal - Online Learning Center Platform

## Executive Summary

This proposal defines a scalable Online Learning Center platform for universities, training institutes, and corporate learning environments. The platform uses **ASP.NET Web Forms**, **SQL Server**, and **Windows Server IIS** to deliver a responsive and secure web application across three main user roles: **Student**, **Instructor**, and **Admin**.

The solution supports complete learning operations including course delivery, live class scheduling, assessments, secure exam access by code, pass/fail automation, required survey collection, and certificate issuance with QR verification.

## Business Goals

- Digitize learning content delivery and tracking.
- Provide role-based portals and dashboards.
- Automate assessment, grading, and certificate lifecycle.
- Enable financial tracking and operational reporting.
- Support management decisions through analytics and KPIs.

## Core Modules

## 1) Public Website
- Home
- About Us
- Contact

## 2) Student Portal
- Dashboard: profile summary, progress, upcoming classes, assignments, announcements, certificate and payment status.
- My Courses: enrollment list, filtering, lesson progress, material downloads, notes/bookmarks, resume learning.
- Assessments: exam list, secure exam access, timer, autosave, auto-submit, history.
- Certificates: eligibility, generated certificates, download and verification links.

## 3) Instructor Portal
- Dashboard with teaching workload and class metrics.
- Course management: curriculum, lessons, files, announcements, publication workflow.
- Class and live-session management with attendance.
- Student interaction, grading, feedback, and escalation.

## 4) Admin Portal
- Master dashboard: users, courses, classes, exams, certificates, finance and alerts.
- User and role management.
- Finance and invoicing workflow.
- Reports and exports.
- Exam governance and certificate lifecycle management.
- Advanced analytics with filterable KPIs.

## 5) Online Exam & Certification Module

- Exam access through validated exam code.
- Identity/eligibility checks and duplicate-attempt prevention.
- Timed exam session (default 60 minutes), autosave, auto-submit.
- Pass/fail evaluation.
- **Required post-pass survey** gate before certificate generation.
- Certificate generation with status lifecycle and QR verification.

## Technical Architecture

- **Presentation Layer:** ASP.NET Web Forms + Bootstrap + JavaScript.
- **Application Layer:** C# services, validation, workflow orchestration.
- **Data Layer:** SQL Server tables, constraints, indexes, reporting views.
- **Hosting:** Windows Server + IIS + SQL Server (on-prem or Azure SQL).

## Security and Compliance Controls

- Role-based access control (RBAC).
- Password hashing, account lockout, session timeout.
- Input validation and parameterized SQL.
- Audit logs for sign-in, grading changes, exam attempts, and finance updates.
- HTTPS-only deployment, secure cookies, anti-forgery measures.

## Non-Functional Requirements

- Responsive UI across desktop/tablet/mobile.
- High availability via IIS app pool best practices.
- Scalable DB indexing for reporting and analytics.
- File storage strategy for videos, resources, and submitted files.
- Backup and disaster recovery policies for database and generated certificates.

## Deliverables in this repository

- Web Forms starter project with role-based page structure.
- SQL schema including exam/survey/certificate dependencies.
- Reporting views for dashboard and executive metrics.
- IIS deployment guideline and operational checklist.
