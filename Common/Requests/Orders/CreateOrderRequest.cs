using System;

namespace Common.Requests.Orders;

public record CreateOrderRequest
{
    public string UserId { get; init; } // Kullanıcı ID'si
    public int CartId { get; init; } // 🔥 Sepet ID'si (Yeni)
    public int? ShippingAddressId { get; init; } // Teslimat adresi
    public int? BillingAddressId { get; init; } // Fatura adresi
}
