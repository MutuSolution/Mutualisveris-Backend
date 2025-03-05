namespace Common.Requests.Products;

public record UpdateProductRequest
(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string SKU,
    bool IsPublic,
    bool IsDeleted
);
