<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">My Courses</h2>
    <p class="section-subtitle mb-3">Filter enrolled courses by active, completed, pending, or expired.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-3"><label class="form-label">Status</label><select class="form-select"><option>Active</option><option>Completed</option><option>Pending</option><option>Expired</option></select></div>
            <div class="col-md-3"><label class="form-label">Category</label><select class="form-select"><option>All</option><option>Technology</option><option>Business</option></select></div>
            <div class="col-md-4"><label class="form-label">Search</label><input class="form-control" placeholder="Course title or instructor" /></div>
            <div class="col-md-2"><button type="button" class="btn btn-gold w-100">Apply</button></div>
        </div>
    </div>

    <div class="table-card p-3">
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Course</th><th>Instructor</th><th>Progress</th><th>Lessons</th><th>Action</th></tr></thead>
            <tbody>
                <tr><td>Applied Data Analytics</td><td>Dr. A. Khan</td><td>68%</td><td>17/25</td><td><a class="btn btn-sm btn-outline-warning" href="#">Resume</a></td></tr>
                <tr><td>Cloud Fundamentals</td><td>Ms. R. Silva</td><td>42%</td><td>9/21</td><td><a class="btn btn-sm btn-outline-warning" href="#">Open</a></td></tr>
                <tr><td>Project Leadership</td><td>Mr. J. Carter</td><td>100%</td><td>18/18</td><td><a class="btn btn-sm btn-outline-warning" href="#">Review</a></td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
