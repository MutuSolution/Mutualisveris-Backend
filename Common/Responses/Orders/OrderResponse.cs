using Common.Responses.Addresses;
using Common.Responses.Identity;
using Common.Responses.Payments;
using Domain.Enums;

namespace Common.Responses.Orders;

public record OrderResponse
{
    public int Id { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; }

    // Kullanıcı Bilgileri
    public UserResponse User { get; init; } // ✅ Kullanıcı detaylarını içeren model

    // Ödeme Bilgileri
    public PaymentResponse? Payment { get; init; }

    // Sipariş Ürünleri
    public List<OrderItemResponse> OrderItems { get; init; } = new();

    // Adres Bilgileri
    public AddressResponse? ShippingAddress { get; init; }
    public AddressResponse? BillingAddress { get; init; }
}
