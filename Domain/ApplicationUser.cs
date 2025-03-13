using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public List<Address> Addresses { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public Cart Cart { get; set; }
}
