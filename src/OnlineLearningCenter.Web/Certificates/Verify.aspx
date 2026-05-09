<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Certificate Verification</h2>
    <p class="section-subtitle mb-3">Verify status by certificate number or QR payload token.</p>

    <div class="dashboard-card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-5"><label class="form-label">Certificate Number</label><input class="form-control" placeholder="CERT-2026-00031" /></div>
            <div class="col-md-5"><label class="form-label">QR Token (optional)</label><input class="form-control" placeholder="Paste QR token" /></div>
            <div class="col-md-2"><button type="button" class="btn btn-gold w-100">Verify</button></div>
        </div>
    </div>

    <div class="table-card p-3">
        <h5 class="text-gold">Verification Result</h5>
        <table class="table table-borderless mb-0">
            <tbody>
                <tr><td style="width:220px;">Certificate Number</td><td>CERT-2026-00031</td></tr>
                <tr><td>Student Name</td><td>Fatima Khaled</td></tr>
                <tr><td>Course Name</td><td>Applied Data Analytics</td></tr>
                <tr><td>Issue Date</td><td>2026-05-03</td></tr>
                <tr><td>Status</td><td><span class="badge badge-soft">Valid</span></td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
