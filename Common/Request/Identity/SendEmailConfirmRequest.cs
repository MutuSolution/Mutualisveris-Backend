using System.ComponentModel.DataAnnotations;

namespace Common.Requests.Identity;

public class SendEmailConfirmRequest
{

    [Required, EmailAddress]
    public string Email { get; set; }

}
