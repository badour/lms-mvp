using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Finance;

public class StudentFee : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int FeePlanId { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public FeePlan? FeePlan { get; set; }
}
