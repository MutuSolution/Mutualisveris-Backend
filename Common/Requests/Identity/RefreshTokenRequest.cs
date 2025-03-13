namespace Common.Requests.Identity;

public record RefreshTokenRequest
{
    public string Token { get; init; }
    public string RefreshToken { get; init; }
}
