namespace Common.Responses.Carts;
public record CartResponse
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string UserId { get; init; } = string.Empty;
    public List<CartItemResponse> Items { get; init; } = new();
}