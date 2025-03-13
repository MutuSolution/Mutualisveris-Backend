namespace Common.Requests.Admin;

// admin -> Update User Details
public record AdminUpdateUserRequest
{
    public string UserId { get; init; } // Zorunlu
    public string? FirstName { get; init; } // Boş olabilir
    public string? LastName { get; init; } // Boş olabilir
    public bool IsActive { get; init; } // Zorunlu
    public bool IsEmailChange { get; init; } = true;
    public string? Email { get; init; } // Boş olabilir
    public bool? EmailConfirmed { get; init; } // Boş olabilir
    public bool IsUserNameChange { get; init; } = true;
    public string? UserName { get; init; } // Boş olabilir
    public bool IsPasswordChange { get; init; }
    public string? Password { get; init; } // Boş olabilir
}
