namespace Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int LikeCount => Likes.Count;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Property
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<ProductImage> Images { get; set; } = new();
    public List<OrderItem> OrderItems { get; set; } = new();
}
