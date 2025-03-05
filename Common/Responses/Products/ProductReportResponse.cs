namespace Common.Responses.Products;

public record ProductReportResponse
(
     int Id,
     int ProductId,
     string? Message,
     bool IsChecked
);
