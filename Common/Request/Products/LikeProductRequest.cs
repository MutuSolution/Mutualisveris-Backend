namespace Common.Requests.Products;

public record LikeProductRequest
{
    public int ProductId { get; init; }
}