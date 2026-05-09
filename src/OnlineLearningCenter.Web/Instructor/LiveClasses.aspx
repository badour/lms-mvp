<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Live Classes</h2>
    <p class="section-subtitle mb-3">Schedule sessions, attach links/materials, and track attendance.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-3"><label class="form-label">Course</label><select class="form-select"><option>Applied Data Analytics</option></select></div>
            <div class="col-md-2"><label class="form-label">Date</label><input class="form-control" type="date" /></div>
            <div class="col-md-2"><label class="form-label">Time</label><input class="form-control" type="time" /></div>
            <div class="col-md-2"><label class="form-label">Duration (min)</label><input class="form-control" type="number" value="60" /></div>
            <div class="col-md-3"><label class="form-label">Meeting Link</label><input class="form-control" placeholder="https://..." /></div>
        </div>
        <div class="mt-3"><button type="button" class="btn btn-gold">Schedule Session</button></div>
    </div>

    <div class="table-card p-3">
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Session</th><th>Status</th><th>Date</th><th>Attendance</th><th>Action</th></tr></thead>
            <tbody>
                <tr><td>Data Analytics - Week 5</td><td>Scheduled</td><td>2026-05-16 10:00</td><td>-</td><td><a class="btn btn-sm btn-outline-warning" href="#">Notify Students</a></td></tr>
                <tr><td>Cloud Fundamentals Intro</td><td>Completed</td><td>2026-05-02 14:00</td><td>87%</td><td><a class="btn btn-sm btn-outline-warning" href="#">Attendance Report</a></td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
