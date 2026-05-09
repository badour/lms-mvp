<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row g-3">
        <div class="col-lg-7">
            <div class="dashboard-card p-4">
                <h2>Contact Us</h2>
                <div class="mb-3">
                    <label class="form-label">Full Name</label>
                    <input type="text" class="form-control" placeholder="Enter your full name" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Email</label>
                    <input type="email" class="form-control" placeholder="name@example.com" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Message</label>
                    <textarea class="form-control" rows="4" placeholder="How can we help?"></textarea>
                </div>
                <button type="button" class="btn btn-gold">Send Message</button>
            </div>
        </div>
        <div class="col-lg-5">
            <div class="dashboard-card p-4 h-100">
                <h4 class="text-gold">Corporate Office</h4>
                <p class="mb-1">Online Learning Center HQ</p>
                <p class="mb-1">Email: support@olc.example</p>
                <p class="mb-1">Phone: +1 555 0100</p>
                <p class="section-subtitle mb-0">Service windows can be configured by branch and timezone.</p>
            </div>
        </div>
    </div>
</asp:Content>
