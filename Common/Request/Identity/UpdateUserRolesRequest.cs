using Common.Responses.Identity;

namespace Common.Requests.Identity;

public record UpdateUserRolesRequest
{
    public string UserId { get; init; }
    public List<UserRoleViewModel> Roles { get; init; }
}
