namespace Common.Responses.Cart;

public record CartResponse
{
    public int CartId { get; init; }
    public List<CartItemResponse> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public int ItemCount => Items.Sum(i => i.Quantity); // 🔥 Toplam ürün sayısı (Adet bazında)
    public DateTime CreatedAt { get; init; } // 🔥 Sepet oluşturulma zamanı
}
