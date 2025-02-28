namespace Domain;

public interface IUser
{
    string Id { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string UserName { get; }
    string Role { get; }
    bool IsActive { get; }
    bool EmailConfirmed { get; }
}