using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ApplicationDbContext dbContext)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request)
    {
        var roleExist = await _roleManager.FindByNameAsync(request.RoleName);
        if (roleExist != null)
            return await ResponseWrapper<string>.FailAsync("[ML28] Role already exists.");

        var newRole = new ApplicationRole
        {
            Name = request.RoleName,
            Description = request.RoleDescription
        };

        var identityResult = await _roleManager.CreateAsync(newRole);
        if (!identityResult.Succeeded)
            return await ResponseWrapper<string>
                .FailAsync(GetIdentityResultErrorDescriptions(identityResult));

        return await ResponseWrapper<string>.SuccessAsync("[ML29] Role created successfully.");
    }

    public async Task<IResponseWrapper> DeleteRoleAsync(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);
        if (roleInDb is null)
            return await ResponseWrapper.FailAsync("[ML30] Role does not exist.");
        if (roleInDb.Name == AppRoles.Admin)
            return await ResponseWrapper.FailAsync("[ML31] Role delete not permitted.");

        var allUsers = await _userManager.Users.ToListAsync();
        foreach (var user in allUsers)
        {
            if (await _userManager.IsInRoleAsync(user, roleInDb.Name))
                return await ResponseWrapper
                    .FailAsync($"[ML32] Role: {roleInDb.Name} is currently assigned to a user.");
        }

        var identityResult = await _roleManager.DeleteAsync(roleInDb);
        if (!identityResult.Succeeded)
            return await ResponseWrapper<string>
                .FailAsync(GetIdentityResultErrorDescriptions(identityResult));

        return await ResponseWrapper<string>.SuccessAsync("[ML33] Role deleted successfully.");
    }

    public async Task<IResponseWrapper> GetRoleByIdAsync(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);
        if (roleInDb is null)
            return await ResponseWrapper.FailAsync("[ML34] No role was found.");

        var mappedRole = _mapper.Map<RoleResponse>(roleInDb);
        return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
    }

    public async Task<IResponseWrapper> GetRolePermissionsAsync(string roleId)
    {
        var roleInDb = await _roleManager.FindByIdAsync(roleId);
        if (roleInDb is null)
            return await ResponseWrapper<RoleClaimResponse>.FailAsync("[ML35] No role was found.");

        var allPermissions = AppPermissions.AllPermissions;
        var roleClaimResponse = new RoleClaimResponse
        {
            Role = new()
            {
                Id = roleId,
                Name = roleInDb.Name,
                Description = roleInDb.Description,
            },
            RoleClaims = new()
        };
        var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId);
        var allPermissionsNames = allPermissions.Select(x => x.Name).ToList();
        var currentRoleClaimsValues = currentRoleClaims.Select(x => x.ClaimValue).ToList();
        var currentlyAssignedRoleClaimsNames = allPermissionsNames
            .Intersect(currentRoleClaimsValues).ToList();

        foreach (var permission in allPermissions)
        {
            var claimAndNameCheck = currentlyAssignedRoleClaimsNames
                .Any(roleClaims => roleClaims == permission.Name);

            roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
            {
                RoleId = roleId,
                ClaimType = AppClaim.Permission,
                ClaimValue = permission.Name,
                Description = permission.Description,
                Group = permission.Group,
                IsAssignedToRole = claimAndNameCheck ? true : false
            });
        }

        return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
    }

    //helper function
    private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
    {
        var roleClaims = await _dbContext.RoleClaims
            .Where(claims => claims.RoleId == roleId).ToListAsync();
        if (roleClaims.Count < 1)
            return new List<RoleClaimViewModel>();

        var mappedRoleClaims = _mapper.Map<List<RoleClaimViewModel>>(roleClaims);
        return mappedRoleClaims;
    }

    public async Task<IResponseWrapper> GetRolesAsync()
    {
        var allRoles = await _roleManager.Roles.ToListAsync();
        if (allRoles.Count < 1)
            return await ResponseWrapper<string>.FailAsync("[ML36] No roles were found.");

        var mappedRoles = _mapper.Map<List<RoleResponse>>(allRoles);
        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(mappedRoles);
    }

    public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoleId);
        if (roleInDb is null)
            return await ResponseWrapper.FailAsync("[ML37] Role does not exist.");
        if (roleInDb.Name == AppRoles.Admin)
            return await ResponseWrapper.FailAsync("[ML38] Role update not permitted.");

        roleInDb.Name = request.RoleName;
        roleInDb.Description = request.RoleDescription;

        var identityResult = await _roleManager.UpdateAsync(roleInDb);

        if (!identityResult.Succeeded)
            return await ResponseWrapper<string>
                .FailAsync(GetIdentityResultErrorDescriptions(identityResult));

        return await ResponseWrapper<string>.SuccessAsync("Role updated successfully");
    }

    private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        var errorDescriptions = new List<string>();
        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);
        }
        return errorDescriptions;
    }

    public async Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoleId);
        if (roleInDb is null)
            return await ResponseWrapper.FailAsync("[ML39] Role does not exist.");
        if (roleInDb.Name == AppRoles.Admin)
            return await ResponseWrapper.FailAsync("[ML40] Changing role permission is not allowed.");
        var permissionsToBeAssigned = request.RoleClaims
            .Where(x => x.IsAssignedToRole == true).ToList();
        var currentlyAssignedClaims = await _roleManager.GetClaimsAsync(roleInDb);
        foreach (var claim in currentlyAssignedClaims)
        {
            await _roleManager.RemoveClaimAsync(roleInDb, claim);
        }
        foreach (var claim in permissionsToBeAssigned)
        {
            var mappedRoleClaim = _mapper.Map<ApplicationRoleClaim>(claim);
            await _dbContext.RoleClaims.AddAsync(mappedRoleClaim);
        }
        await _dbContext.SaveChangesAsync();
        return await ResponseWrapper<string>.SuccessAsync("[ML41] Role permissions updated successfully.");
    }
}
