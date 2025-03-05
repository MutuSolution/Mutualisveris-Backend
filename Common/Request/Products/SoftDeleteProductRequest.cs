namespace Common.Requests.Products;

public record SoftDeleteProductRequest
{
    public int ProductId { get; init; }
}
