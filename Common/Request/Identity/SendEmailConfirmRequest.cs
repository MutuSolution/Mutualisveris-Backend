using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Identity;

public record SendEmailConfirmRequest
{

    [Required, EmailAddress]
    public string Email { get; init; }

}
