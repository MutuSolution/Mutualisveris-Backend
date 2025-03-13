namespace Common.Requests.Identity;

public record UpdateUserRequest
{
    public string UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string UserName { get; init; }
}
