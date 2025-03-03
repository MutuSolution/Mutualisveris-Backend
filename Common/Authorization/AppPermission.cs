using System.Collections.ObjectModel;

namespace Common.Authorization;

public record AppPermission(string Feature, string Action, string Group, string Description, bool IsBasic = false)
{
    public string Name => NameFor(Feature, Action);

    public static string NameFor(string feature, string action)
    {
        return $"Permissions.{feature}.{action}";
    }
}

public class AppPermissions
{
    private static readonly AppPermission[] _all = new AppPermission[]
    {
            new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
            new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users", IsBasic: true),
            new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users", IsBasic: true),
            new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),

            new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read User Roles", IsBasic: true),
            new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update User Roles"),

            new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Roles", IsBasic: true),
            new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
            new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
            new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),

            new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess,
                "Read Role Claims/Permissions", IsBasic: true),
            new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess,
                "Update Role Claims/Permissions"),

            new(AppFeature.Products, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Products", IsBasic: true),
            new(AppFeature.Products, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Products"),
            new(AppFeature.Products, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Products"),
            new(AppFeature.Products, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Products"),

        new(AppFeature.Categories, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Categories", IsBasic: true),
            new(AppFeature.Categories, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Categories"),
            new(AppFeature.Categories, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Categories"),
            new(AppFeature.Categories, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Categories")
    };

    public static IReadOnlyList<AppPermission> AdminPermissions { get; } =
       new ReadOnlyCollection<AppPermission>(_all).ToArray();
    //  isbasic olmayan admin
    //  new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray()); 

    public static IReadOnlyList<AppPermission> BasicPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());

    public static IReadOnlyList<AppPermission> AllPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all);
}