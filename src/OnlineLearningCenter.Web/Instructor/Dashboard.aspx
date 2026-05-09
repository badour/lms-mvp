<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Instructor Dashboard</h2>
    <p class="section-subtitle mb-3">Course analytics, grading queue, live sessions, and student engagement alerts.</p>

    <div class="row g-3 mb-3">
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Assigned Courses</div><div class="metric">6</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Total Students</div><div class="metric">244</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Pending Grading</div><div class="metric">18</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Live Sessions This Week</div><div class="metric">9</div></div></div>
    </div>

    <div class="quick-links mb-3">
        <a href="/Instructor/Courses.aspx">Course Management</a>
        <a href="/Instructor/LiveClasses.aspx">Live Classes</a>
        <a href="/Student/Assessments.aspx">Assessments</a>
    </div>

    <div class="table-card p-3">
        <h5 class="text-gold">Low Progress Watchlist</h5>
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Student</th><th>Course</th><th>Completion</th><th>Attendance</th><th>Action</th></tr></thead>
            <tbody>
                <tr><td>Sara Mahmoud</td><td>Cloud Fundamentals</td><td>22%</td><td>58%</td><td><a class="btn btn-sm btn-outline-warning" href="#">Send Support Plan</a></td></tr>
                <tr><td>Ali Rahman</td><td>Data Analytics</td><td>30%</td><td>61%</td><td><a class="btn btn-sm btn-outline-warning" href="#">Message</a></td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
