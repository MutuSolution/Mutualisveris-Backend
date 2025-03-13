namespace Common.Requests.Orders;
public record OrderItemRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}