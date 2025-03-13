namespace Common.Requests.Identity;

public record ChangeEmailRequest
{
    public string Email { get; init; }
}
