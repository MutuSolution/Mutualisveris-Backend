namespace Common.Requests.Product.Report;

public record ProductReportIsCheckedRequest
{
    public int ReportId { get; init; }
    public bool IsChecked { get; init; }
}
