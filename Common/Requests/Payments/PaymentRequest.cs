using Domain.Enums;

namespace Common.Requests.Payments;

public record PaymentRequest
{
    public decimal Amount { get; init; } // 🔥 Ödeme miktarı
    public PaymentMethod Method { get; init; } // 🔥 Ödeme yöntemi (Kredi Kartı, PayPal vb.)
    public DateTime PaymentDate { get; init; } = DateTime.UtcNow; // 🔥 Ödeme tarihi (varsayılan olarak şu an)
    public PaymentStatus Status { get; init; } = PaymentStatus.Pending; // 🔥 Ödeme durumu (Varsayılan: Bekliyor)
    public int OrderId { get; init; } // 🔥 Ödeme hangi siparişe ait?
}
