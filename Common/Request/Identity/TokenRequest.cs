namespace Common.Requests.Identity;

public record TokenRequest
{
    public string Email { get; init; }
    public string Password { get; init; }
}
