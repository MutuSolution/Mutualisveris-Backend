namespace Common.Responses.Products;

public record ProductImageResponse
(
    int Id,
    string ImageUrl,
    bool IsMain
);
