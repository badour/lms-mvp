<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-1">Finance Management</h2>
    <p class="section-subtitle mb-3">Manage prices, payments, invoices, refunds, discounts, and revenue reports.</p>

    <div class="row g-3 mb-3">
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Paid Revenue</div><div class="metric">$84,120</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Pending Payments</div><div class="metric">$12,450</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Refunded</div><div class="metric">$3,210</div></div></div>
        <div class="col-md-3"><div class="dashboard-card p-3"><div class="section-subtitle">Outstanding</div><div class="metric">$6,900</div></div></div>
    </div>

    <div class="table-card p-3">
        <table class="table table-borderless align-middle mb-0">
            <thead><tr><th>Invoice</th><th>Student</th><th>Amount</th><th>Method</th><th>Status</th><th>Date</th></tr></thead>
            <tbody>
                <tr><td>INV-1001</td><td>Hassan Omar</td><td>$550.00</td><td>Card</td><td>Paid</td><td>2026-05-08</td></tr>
                <tr><td>INV-1002</td><td>Dina Samir</td><td>$300.00</td><td>Bank Transfer</td><td>Pending</td><td>2026-05-08</td></tr>
                <tr><td>INV-1003</td><td>Waleed Nabil</td><td>$450.00</td><td>Card</td><td>Refunded</td><td>2026-05-07</td></tr>
            </tbody>
        </table>
    </div>
</asp:Content>
