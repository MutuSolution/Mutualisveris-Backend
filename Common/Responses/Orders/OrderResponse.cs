namespace Common.Responses.Orders;

public record OrderResponse
(
    int Id,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status, // Enum ise string'e dönüştürülebilir
    string UserId,
    List<OrderItemResponse> OrderItems,
    PaymentResponse Payment
);
