namespace Common.Requests.Identity;

public record ChangeUserStatusRequest
{
    public string UserId { get; init; }
    public bool Activate { get; init; }
}
