using Common.Requests.Payments;

namespace Common.Request.Orders;
public record CreateOrderRequest
{
    public string UserId { get; init; } // 🔥 Kullanıcı kimliği (zorunlu)
    public int? ShippingAddressId { get; init; } // 🔥 Opsiyonel
    public int? BillingAddressId { get; init; }  // 🔥 Opsiyonel
    public List<OrderItemRequest> OrderItems { get; init; } = new(); // 🔥 Sipariş edilen ürünler
    public PaymentRequest? Payment { get; init; } // 🔥 Ödeme bilgisi (opsiyonel olabilir)
}