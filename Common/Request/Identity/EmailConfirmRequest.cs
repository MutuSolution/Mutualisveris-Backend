using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Identity;

public class EmailConfirmRequest
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }

}
