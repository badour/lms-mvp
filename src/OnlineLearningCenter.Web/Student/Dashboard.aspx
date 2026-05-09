<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Student Dashboard</h2>
    <p class="section-subtitle mb-3">Profile summary, progress, upcoming classes, exams, assignments, and certificate eligibility.</p>

    <div class="row g-3 mb-3">
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Enrolled Courses</div><div class="metric">8</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Pending Assignments</div><div class="metric">3</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Upcoming Exams</div><div class="metric">2</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Certificate Ready</div><div class="metric">4</div></div></div>
    </div>

    <div class="dashboard-card p-3 mb-3">
        <h5 class="text-gold">Quick Access</h5>
        <div class="quick-links">
            <a href="/Student/Courses.aspx">My Courses</a>
            <a href="/Student/Assessments.aspx">Exams & Assessments</a>
            <a href="/Student/Certificates.aspx">Certificates</a>
            <a href="/Contact.aspx">Support</a>
        </div>
    </div>

    <div class="table-card p-3">
        <h5 class="text-gold">Announcements</h5>
        <table class="table table-borderless align-middle mb-0">
            <thead>
                <tr><th>Announcement</th><th>Date</th><th>Source</th></tr>
            </thead>
            <tbody>
                <tr><td>New live class schedule published for Data Analytics.</td><td>2026-05-12</td><td>Instructor</td></tr>
                <tr><td>Exam code release for Cloud Fundamentals.</td><td>2026-05-14</td><td>Exam Office</td></tr>
                <tr><td>Survey completion required before certificate download.</td><td>2026-05-15</td><td>Administration</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
