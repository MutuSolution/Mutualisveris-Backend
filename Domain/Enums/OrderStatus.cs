namespace Domain.Enums;
public enum OrderStatus
{
    Pending,       // Sipariş alındı, işlem bekleniyor
    Processing,    // Sipariş hazırlanıyor
    Shipped,       // Kargoya verildi
    Delivered,     // Teslim edildi
    Cancelled      // İptal edildi
}
