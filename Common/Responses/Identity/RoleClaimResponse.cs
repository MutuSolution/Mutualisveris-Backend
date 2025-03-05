namespace Common.Responses.Identity;

public record RoleClaimResponse
(
     RoleResponse Role,
     List<RoleClaimViewModel> RoleClaims
);
