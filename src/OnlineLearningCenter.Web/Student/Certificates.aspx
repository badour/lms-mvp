<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Certificates</h2>
    <p class="section-subtitle mb-3">Certificates are generated after pass status and required survey completion.</p>

    <div class="table-card p-3">
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Certificate #</th><th>Course</th><th>Issue Date</th><th>Status</th><th>Actions</th></tr></thead>
            <tbody>
                <tr><td>CERT-2026-00027</td><td>Project Leadership</td><td>2026-04-30</td><td>Valid</td><td><a class="btn btn-sm btn-outline-warning" href="#">Download PDF</a></td></tr>
                <tr><td>CERT-2026-00031</td><td>Applied Data Analytics</td><td>2026-05-03</td><td>Valid</td><td><a class="btn btn-sm btn-outline-warning" href="#">Share Link</a></td></tr>
                <tr><td>-</td><td>Cloud Fundamentals</td><td>-</td><td>Not Eligible</td><td><span class="badge badge-soft">Pass exam + survey required</span></td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
