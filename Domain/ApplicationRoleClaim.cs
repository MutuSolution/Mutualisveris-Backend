using Microsoft.AspNetCore.Identity;

namespace Domain;
public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public string Description { get; set; }
    public string Group { get; set; }
}
