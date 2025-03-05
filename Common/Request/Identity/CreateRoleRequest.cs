namespace Common.Requests.Identity;

public record CreateRoleRequest
{
    public string RoleName { get; init; }
    public string RoleDescription { get; init; }
}
