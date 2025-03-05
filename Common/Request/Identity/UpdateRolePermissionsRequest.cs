using Common.Responses.Identity;

namespace Common.Requests.Identity;

public record UpdateRolePermissionsRequest
{
    public string RoleId { get; init; }
    public List<RoleClaimViewModel> RoleClaims { get; init; }
}
