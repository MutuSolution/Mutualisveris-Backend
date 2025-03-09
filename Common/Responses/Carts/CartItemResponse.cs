namespace Common.Responses.Carts;

public record CartItemResponse
{
    public int Id { get; init; }
    public int Quantity { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
}