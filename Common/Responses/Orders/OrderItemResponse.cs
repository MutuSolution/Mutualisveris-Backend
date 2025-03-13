namespace Common.Responses.Orders;

public record OrderItemResponse
{
    public int ProductId { get; init; }
    public string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice; // 🔥 Toplam fiyat
    public string SKU { get; init; } = string.Empty; // 🔥 Ürün stok kodu
    public string ImageUrl { get; init; } // 🔥 Ürün ana resmi
}
