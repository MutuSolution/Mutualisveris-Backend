namespace Common.Responses.Identity;

public record RoleClaimResponse
{
    public RoleResponse Role { get; init; }
    public List<RoleClaimViewModel> RoleClaims { get; init; } = new();
}