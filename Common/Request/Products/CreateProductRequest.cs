namespace Common.Requests.Products;

public record CreateProductRequest
(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string SKU,
    int CategoryId = 20,
    bool IsPublic = false
);
