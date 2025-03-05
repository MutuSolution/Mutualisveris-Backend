namespace Common.Responses.Products;

public record ProductResponse
(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string SKU,
    bool IsPublic,
    bool IsDeleted,
    int LikeCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string CategoryName,
    List<string> ImageUrls,
    int OrderItemCount,
    bool IsLiked = false
);
