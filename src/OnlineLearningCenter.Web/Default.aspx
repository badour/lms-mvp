<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hero mb-4">
        <span class="badge badge-soft mb-3">Executive Summary</span>
        <h1 class="display-6 fw-bold">Digital Learning Platform for Universities, Training Institutes, and Corporate Learning</h1>
        <p class="lead section-subtitle">
            Deliver responsive online learning with role-based dashboards, course lifecycle management,
            secure exam workflows, mandatory survey gating, and QR-based certificate verification.
        </p>
        <div class="d-flex flex-wrap gap-2">
            <a href="/Student/Dashboard.aspx" class="btn btn-gold">Student Portal</a>
            <a href="/Instructor/Dashboard.aspx" class="btn btn-outline-warning">Instructor Portal</a>
            <a href="/Admin/Dashboard.aspx" class="btn btn-outline-warning">Admin Portal</a>
        </div>
    </section>

    <div class="row g-3">
        <div class="col-md-4">
            <div class="dashboard-card p-3 h-100">
                <h5 class="text-gold">Student Experience</h5>
                <p class="mb-0 section-subtitle">Courses, exams, progress tracking, certificates, and subscription status in one portal.</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="dashboard-card p-3 h-100">
                <h5 class="text-gold">Instructor Tools</h5>
                <p class="mb-0 section-subtitle">Curriculum management, live classes, grading, feedback, and intervention tracking.</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="dashboard-card p-3 h-100">
                <h5 class="text-gold">Admin Control</h5>
                <p class="mb-0 section-subtitle">Users, finance, reports, exams, certificates, and analytics for executive decisions.</p>
            </div>
        </div>
    </div>
</asp:Content>
