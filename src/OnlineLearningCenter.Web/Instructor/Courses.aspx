<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Course Management</h2>
    <p class="section-subtitle mb-3">Create courses, upload materials, structure curriculum, and submit for approval.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-4"><label class="form-label">Course Title</label><input class="form-control" placeholder="New course title" /></div>
            <div class="col-md-2"><label class="form-label">Category</label><select class="form-select"><option>Technology</option></select></div>
            <div class="col-md-2"><label class="form-label">Level</label><select class="form-select"><option>Intermediate</option></select></div>
            <div class="col-md-2"><label class="form-label">Duration (hrs)</label><input class="form-control" type="number" /></div>
            <div class="col-md-2"><button type="button" class="btn btn-gold w-100">Create</button></div>
        </div>
    </div>

    <div class="table-card p-3">
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Course</th><th>Status</th><th>Students</th><th>Completion Rate</th><th>Approval</th></tr></thead>
            <tbody>
                <tr><td>Applied Data Analytics</td><td>Published</td><td>118</td><td>71%</td><td>Approved</td></tr>
                <tr><td>Cloud Fundamentals</td><td>Draft</td><td>0</td><td>-</td><td>Pending Admin Review</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
