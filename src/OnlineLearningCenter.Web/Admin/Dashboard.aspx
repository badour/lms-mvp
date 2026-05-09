<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Admin Dashboard</h2>
    <p class="section-subtitle mb-3">Central control panel for users, finance, exams, certificates, and analytics.</p>

    <div class="row g-3 mb-3">
        <div class="col-md-2"><div class="dashboard-card p-3"><div class="section-subtitle">Students</div><div class="metric">1,240</div></div></div>
        <div class="col-md-2"><div class="dashboard-card p-3"><div class="section-subtitle">Instructors</div><div class="metric">62</div></div></div>
        <div class="col-md-2"><div class="dashboard-card p-3"><div class="section-subtitle">Courses</div><div class="metric">94</div></div></div>
        <div class="col-md-2"><div class="dashboard-card p-3"><div class="section-subtitle">Exams</div><div class="metric">186</div></div></div>
        <div class="col-md-2"><div class="dashboard-card p-3"><div class="section-subtitle">Certificates</div><div class="metric">3,420</div></div></div>
        <div class="col-md-2"><div class="dashboard-card p-3"><div class="section-subtitle">Revenue</div><div class="metric">$84k</div></div></div>
    </div>

    <div class="quick-links mb-3">
        <a href="/Admin/Users.aspx">User Management</a>
        <a href="/Admin/Finance.aspx">Finance</a>
        <a href="/Exams/Access.aspx">Exam Monitoring</a>
        <a href="/Certificates/Verify.aspx">Certificate Validation</a>
    </div>

    <div class="table-card p-3">
        <h5 class="text-gold">System Alerts</h5>
        <table class="table table-borderless mb-0">
            <thead><tr><th>Alert</th><th>Priority</th><th>Module</th><th>Status</th></tr></thead>
            <tbody>
                <tr><td>Pending course approvals</td><td>Medium</td><td>Course Management</td><td>12 pending</td></tr>
                <tr><td>Refund requests awaiting decision</td><td>High</td><td>Finance</td><td>5 pending</td></tr>
                <tr><td>Exam sessions auto-submitted today</td><td>Info</td><td>Exams</td><td>31 sessions</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
