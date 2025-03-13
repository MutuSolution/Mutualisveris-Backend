namespace Common.Requests.Identity;

public record UpdateRoleRequest
{
    public string RoleId { get; init; }
    public string RoleName { get; init; }
    public string RoleDescription { get; init; }
}
