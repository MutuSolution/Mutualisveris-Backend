namespace Common.Responses.Orders;

public record PaymentResponse
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; } = string.Empty; // "CreditCard", "PayPal", "BankTransfer"
    public DateTime PaymentDate { get; init; }
    public string Status { get; init; } = string.Empty; // "Pending", "Completed", "Failed"
}