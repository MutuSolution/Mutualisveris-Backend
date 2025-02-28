namespace Common.Requests.Products;

public class ProductReportRequest
{
    public int ProductId { get; init; }
    public string Message { get; init; } = string.Empty;
}
