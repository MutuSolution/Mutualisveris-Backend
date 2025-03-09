namespace Common.Responses.Orders;

public record OrderItemResponse
{
    public int Id { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
}