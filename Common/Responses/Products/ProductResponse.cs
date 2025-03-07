namespace Common.Responses.Products;

public record ProductResponse
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public string SKU { get; init; }
    public bool IsPublic { get; init; }
    public bool IsDeleted { get; init; }
    public int LikeCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string CategoryName { get; init; }
    public List<string> ImageUrls { get; init; }
    public int OrderItemCount { get; init; }
    public bool IsLiked { get; init; } = false;
}
