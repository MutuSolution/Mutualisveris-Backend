namespace Common.Responses.Identity;

public record UserResponse
(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    bool IsActive,
    string Role,
    bool EmailConfirmed
);
