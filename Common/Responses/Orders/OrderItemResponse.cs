public record OrderItemResponse
{
    public int ProductId { get; init; }
    public string ProductName { get; set; } = string.Empty;  // 🔥 Ürün adı boş gelmesin
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice; // ✅ Toplam fiyat
    public string SKU { get; set; } = "N/A"; // ✅ Varsayılan SKU
    public string? ImageUrl { get; set; } // ✅ Ürün resmi
}
