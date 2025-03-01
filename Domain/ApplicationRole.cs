using Microsoft.AspNetCore.Identity;

namespace Domain;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
}
