using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Identity;

public class TokenRequest
{
    public string UserName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
