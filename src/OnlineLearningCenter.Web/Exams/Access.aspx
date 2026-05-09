<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Exam Access Panel</h2>
    <p class="section-subtitle mb-3">Validate exam code, verify eligibility, and start secure timed session.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-4"><label class="form-label">Exam Code</label><input class="form-control" placeholder="EXAM-XXXX-YYYY" /></div>
            <div class="col-md-3"><label class="form-label">Student ID</label><input class="form-control" placeholder="STU-001" /></div>
            <div class="col-md-3"><label class="form-label">Identity Validation</label><select class="form-select"><option>National ID</option><option>Institution ID</option></select></div>
            <div class="col-md-2"><button class="btn btn-gold w-100" type="button">Start Exam</button></div>
        </div>
    </div>

    <div class="table-card p-3">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="text-gold m-0">Sample Exam Session Header</h5>
            <div>
                Remaining Time: <strong data-exam-countdown data-duration-minutes="60">60:00</strong>
            </div>
        </div>
        <table class="table table-borderless mb-0">
            <tbody>
                <tr><td style="width:180px;">Student Full Name</td><td>Ahmed Hassan</td></tr>
                <tr><td>Student ID</td><td>STU-00125</td></tr>
                <tr><td>Course Name</td><td>Applied Data Analytics</td></tr>
                <tr><td>Exam Title</td><td>Final Assessment</td></tr>
                <tr><td>Question Status</td><td>Answered 6 / 20</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
