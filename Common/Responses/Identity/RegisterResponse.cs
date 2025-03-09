namespace Common.Responses.Identity;

public record RegisterResponse
{
    public string Message { get; init; } = string.Empty;
}