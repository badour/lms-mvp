<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">User Management</h2>
    <p class="section-subtitle mb-3">Create and maintain Admin, Student, Instructor, Finance Officer, Exam Officer, and Viewer users.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-3"><label class="form-label">Search</label><input class="form-control" placeholder="Name, ID, email" /></div>
            <div class="col-md-2"><label class="form-label">Role</label><select class="form-select"><option>All Roles</option></select></div>
            <div class="col-md-2"><label class="form-label">Department</label><input class="form-control" /></div>
            <div class="col-md-2"><label class="form-label">Status</label><select class="form-select"><option>Active</option><option>Locked</option></select></div>
            <div class="col-md-3 d-flex gap-2"><button type="button" class="btn btn-gold flex-grow-1">Filter</button><button type="button" class="btn btn-outline-warning flex-grow-1">Export</button></div>
        </div>
    </div>

    <div class="table-card p-3">
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Name</th><th>Email</th><th>Role</th><th>Status</th><th>Last Login</th></tr></thead>
            <tbody>
                <tr><td>Nora Saleh</td><td>nora@olc.example</td><td>Admin</td><td>Active</td><td>2026-05-09 08:31</td></tr>
                <tr><td>Rami Farid</td><td>rami@olc.example</td><td>Instructor</td><td>Active</td><td>2026-05-09 07:50</td></tr>
                <tr><td>Mariam Adel</td><td>mariam@olc.example</td><td>Student</td><td>Locked</td><td>2026-05-06 18:20</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
