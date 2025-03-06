namespace Common.Requests.Products;

public record CreateProductRequest
{
public string Name { get; init; }
public string Description { get; init; }
public decimal Price { get; init; }
public int StockQuantity { get; init; }
public int CategoryId { get; init; }
public string SKU { get; init; }
public bool IsPublic { get; init; }

}
 