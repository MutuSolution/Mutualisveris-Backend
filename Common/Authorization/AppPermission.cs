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
        //Users
            new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
            new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users", IsBasic: true),
            new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users", IsBasic: true),
            new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),

        //UserRoles
            new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read User Roles", IsBasic: true),
            new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update User Roles"),

        //Roles
            new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
            new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Roles", IsBasic: true),
            new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
            new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),

        //Role Claims/Permissions
            new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess,
                "Read Role Claims/Permissions", IsBasic: true),
            new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess,
                "Update Role Claims/Permissions"),

        //Products
            new(AppFeature.Products, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Products"),
            new(AppFeature.Products, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Products", IsBasic: true),
            new(AppFeature.Products, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Products"),
            new(AppFeature.Products, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Products"),

        //Categories
            new(AppFeature.Categories, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Categories"),
            new(AppFeature.Categories, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Categories", IsBasic: true),
            new(AppFeature.Categories, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Categories"),
            new(AppFeature.Categories, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Categories"),

        //Carts (cart için normalde izne gerek yok anonymous verdik ama bu yinede elimizde bulunsun.)
            new(AppFeature.Carts, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Carts", IsBasic: true),
            new(AppFeature.Carts, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Carts", IsBasic: true),
            new(AppFeature.Carts, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Carts", IsBasic: true),
            new(AppFeature.Carts, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Carts", IsBasic : true),

        //Addresses
            new(AppFeature.Addresses, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Addresses", IsBasic: true),
            new(AppFeature.Addresses, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Addresses", IsBasic: true),
            new(AppFeature.Addresses, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Addresses", IsBasic: true),
            new(AppFeature.Addresses, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Addresses", IsBasic : true),

        //Orders
            new(AppFeature.Orders, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Orders", IsBasic: true),
            new(AppFeature.Orders, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Order", IsBasic: true),
            new(AppFeature.Orders, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Orders", IsBasic: true),
            new(AppFeature.Orders, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Orders", IsBasic : true),

        //Payments
            new(AppFeature.Payments, AppAction.Create, AppRoleGroup.ManagementHierarchy,
                "Create Payments", IsBasic: true),
            new(AppFeature.Payments, AppAction.Read, AppRoleGroup.ManagementHierarchy,
                "Read Payments", IsBasic: true),
            new(AppFeature.Payments, AppAction.Update, AppRoleGroup.ManagementHierarchy,
                "Update Payments", IsBasic: true),
            new(AppFeature.Payments, AppAction.Delete, AppRoleGroup.ManagementHierarchy,
                "Delete Payments", IsBasic : true)
    };

    //For admin
    public static IReadOnlyList<AppPermission> AdminPermissions { get; } =
       new ReadOnlyCollection<AppPermission>(_all).ToArray();

    //For user
    public static IReadOnlyList<AppPermission> BasicPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());

    //For settings
    public static IReadOnlyList<AppPermission> AllPermissions { get; } =
        new ReadOnlyCollection<AppPermission>(_all);
}