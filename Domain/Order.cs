using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; } // ✅ Kullanıcı ilişkisini ekledik

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Relationships
    public List<OrderItem> OrderItems { get; set; } = new();
    public Payment? Payment { get; set; }

    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    public Address? ShippingAddress { get; set; }
    public Address? BillingAddress { get; set; }
}
