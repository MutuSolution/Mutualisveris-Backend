namespace Domain;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Relationships
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public Payment Payment { get; set; }

    // Addresses for order
    public int? ShippingAddressId { get; set; }  // Yeni ekleme
    public int? BillingAddressId { get; set; }   // Yeni ekleme
    public Address ShippingAddress { get; set; }  // Yeni ekleme
    public Address BillingAddress { get; set; }   // Yeni ekleme
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}