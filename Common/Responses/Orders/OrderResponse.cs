using Common.Responses.Addresses;
using Common.Responses.Payments;
using Domain.Enums;

namespace Common.Responses.Orders;

public record OrderResponse
{
    public int Id { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; }

    // Kullanıcı Bilgileri (Yalnızca ID ve Ad)
    public string UserId { get; init; }
    public string UserName { get; init; }

    // Ödeme Bilgileri
    public PaymentResponse? Payment { get; init; }

    // Sipariş Ürünleri
    public List<OrderItemResponse> OrderItems { get; init; } = new();

    // Adres Bilgileri
    public AddressResponse? ShippingAddress { get; init; }
    public AddressResponse? BillingAddress { get; init; }
}
