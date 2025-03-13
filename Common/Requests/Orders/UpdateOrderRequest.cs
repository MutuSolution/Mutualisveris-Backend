using System;
using System.Collections.Generic;
using Common.Requests.Orders;
using Domain.Enums;

namespace Common.Requests.Orders;

public record UpdateOrderRequest
{
    public int OrderId { get; init; } // 🔥 Güncellenecek siparişin ID'si
    public OrderStatus? Status { get; init; } // 🔥 Sipariş durumu değişebilir (Nullable)
    public int? ShippingAddressId { get; init; } // 🔥 Yeni teslimat adresi (Nullable)
    public int? BillingAddressId { get; init; } // 🔥 Yeni fatura adresi (Nullable)
    public List<OrderItemRequest>? OrderItems { get; init; } // 🔥 Ürün güncelleme
}
