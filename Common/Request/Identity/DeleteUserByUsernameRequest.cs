namespace Common.Requests.Identity;

public record DeleteUserByUsernameRequest
{
    public string UserName { get; init; }
}
