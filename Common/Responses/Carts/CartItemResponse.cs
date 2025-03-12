namespace Common.Responses.Cart;

public record CartItemResponse
{
    public int ProductId { get; init; }
    public string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public int Stock { get; init; } // 🔥 Ürünün mevcut stok durumu
    public string SKU { get; init; } = string.Empty; // 🔥 Ürünün SKU kodu (Varsayılan boş)
    public string? ImageUrl { get; init; } // 🔥 Ürünün ana resmi (Opsiyonel)

    // 🔥 `TotalPrice` hesaplamasını buraya koyduk, böylece hata oluşmaz!
    public decimal TotalPrice => Quantity * UnitPrice;
}
