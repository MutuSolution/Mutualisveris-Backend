using Domain;
using Microsoft.AspNetCore.Identity;

namespace Domain;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; } = "Basic";
    public DateTime RefreshTokenExpiryDate { get; set; }
    public bool IsActive { get; set; }


}
