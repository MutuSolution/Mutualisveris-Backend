namespace Common.Responses.Products;

public record ProductImageResponse
{
    public int Id { get; init; }
    public string ImageUrl { get; init; }
    public bool IsMain { get; init; }
};
