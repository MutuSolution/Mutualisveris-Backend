namespace Common.Responses.Orders;

public record OrderResponse
{
    public int Id { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty; // Enum ise string'e dönüştürülebilir
    public string UserId { get; init; } = string.Empty;
    public List<OrderItemResponse> OrderItems { get; init; } = new();
    public PaymentResponse Payment { get; init; } = new();
}