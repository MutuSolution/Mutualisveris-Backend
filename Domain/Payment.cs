namespace Domain;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Relationships
    public int OrderId { get; set; }
    public Order Order { get; set; }
}

public enum PaymentMethod
{
    CreditCard,
    PayPal,
    BankTransfer
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}
