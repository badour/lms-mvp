<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Assessments</h2>
    <p class="section-subtitle mb-3">Secure exam code validation, timer, autosave, and exam history.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-4"><label class="form-label">Exam Code</label><input class="form-control" placeholder="Enter secure exam code" /></div>
            <div class="col-md-3"><label class="form-label">Student ID</label><input class="form-control" placeholder="Student ID" /></div>
            <div class="col-md-3"><label class="form-label">Course</label><select class="form-select"><option>Assigned Course</option></select></div>
            <div class="col-md-2"><button type="button" class="btn btn-gold w-100">Validate</button></div>
        </div>
    </div>

    <div class="table-card p-3">
        <h5 class="text-gold">Exam Attempt History</h5>
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Exam</th><th>Attempt</th><th>Score</th><th>Status</th><th>Submission</th></tr></thead>
            <tbody>
                <tr><td>Data Analytics Midterm</td><td>#1</td><td>78</td><td>Passed</td><td>Submitted</td></tr>
                <tr><td>Cloud Fundamentals Quiz</td><td>#1</td><td>54</td><td>Failed</td><td>AutoSubmitted</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
