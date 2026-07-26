namespace SchoolLMS.Domain.Entities.Finance;

public class PaymentAllocation
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public int StudentFeeId { get; set; }
    public decimal Amount { get; set; }

    public Payment? Payment { get; set; }
    public StudentFee? StudentFee { get; set; }
}
