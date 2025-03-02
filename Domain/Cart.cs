namespace Domain;

public class Cart
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public List<CartItem> Items { get; set; } = new();
}