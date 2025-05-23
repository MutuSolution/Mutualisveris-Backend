﻿using Common.Authorization;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ApplicationDbSeeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbSeeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task SeedDatabaseAsync()
    {
        await CheckAndApplyPendingMigrationAsync();
        await SeedRolesAsync();

        await SeedBasicUserAsync();

        await SeedAdminUserAsync();
    }

    private async Task CheckAndApplyPendingMigrationAsync()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
    }

    private async Task SeedAdminUserAsync()
    {
        string adminUserName = AppCredentials
            .Email[..AppCredentials.Email.IndexOf('@')].ToLowerInvariant();
        var adminUser = new ApplicationUser
        {
            FirstName = "Yunus",
            LastName = "Gündüz",
            Email = AppCredentials.Email,
            UserName = adminUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
            NormalizedUserName = adminUserName.ToUpperInvariant(),
            IsActive = true
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == AppCredentials.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.Password);
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic)
            && !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            await _userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
    }

    private async Task SeedBasicUserAsync()
    {
        var basicUser = new ApplicationUser
        {
            FirstName = "Mustafa",
            LastName = "Toprak",
            Email = "mustafatoprak@gmail.com",
            UserName = "mustafatoprak",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            NormalizedEmail = "MUSTAFATOPRAK@GMAIL.COM",
            NormalizedUserName = "MUSTAFATOPRAK",
            IsActive = true
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == "mustafatoprak@gmail.com"))
        {
            var password = new PasswordHasher<ApplicationUser>();
            basicUser.PasswordHash = password.HashPassword(basicUser, AppCredentials.Password);
            await _userManager.CreateAsync(basicUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(basicUser, AppRoles.Basic))
        {
            await _userManager.AddToRoleAsync(basicUser, AppRoles.Basic);
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role."
                };

                await _roleManager.CreateAsync(role);
            }

            if (roleName == AppRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
            }
            else if (roleName == AppRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationRole role, IReadOnlyList<AppPermission> permissions)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
            {
                await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
