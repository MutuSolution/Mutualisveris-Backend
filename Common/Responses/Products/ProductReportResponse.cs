namespace Common.Responses.Products;

public class ProductReportResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string? Message { get; init; }
    public bool IsChecked { get; init; }
}
