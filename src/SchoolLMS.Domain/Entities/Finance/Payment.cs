using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Finance;

public class Payment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string Method { get; set; } = "Cash";
    public string? ReferenceNumber { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
    public string? ReceivedByUserId { get; set; }
    public string? Notes { get; set; }

    public ICollection<PaymentAllocation> Allocations { get; set; } = new List<PaymentAllocation>();
}
