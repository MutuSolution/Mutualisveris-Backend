namespace Common.Responses.Orders;

public record OrderItemResponse
(
    int Id,
    int Quantity,
    decimal UnitPrice,
    int ProductId,
    string ProductName // Ürünün adı, sipariş detaylarında kullanışlıdır.
);
