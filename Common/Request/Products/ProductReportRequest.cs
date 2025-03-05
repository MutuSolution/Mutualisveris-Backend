namespace Common.Requests.Products;

public record ProductReportRequest
{
    public int ProductId { get; init; }
    public string Message { get; init; } = string.Empty;
}
