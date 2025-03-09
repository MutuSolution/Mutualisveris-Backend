namespace Common.Responses.Identity;
public record UserResponse
{
    public string Id { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Role { get; init; } = string.Empty;
    public bool EmailConfirmed { get; init; }
}