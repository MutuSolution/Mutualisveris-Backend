using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Identity;

public record EmailConfirmRequest
{
    [Required, EmailAddress]
    public string Email { get; init; }

    [Required]
    public string Code { get; init; }

}
