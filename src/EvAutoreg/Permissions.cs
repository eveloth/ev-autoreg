using DataAccessLibrary.DisplayModels;

namespace EvAutoreg;

public static class Permissions
{
    private static readonly List<PermissionModel> permissions = new List<PermissionModel>
    {
        new() { PermissionName = "ResetUserPasswords", Description = "Can reset a password of any user"},
        new() { PermissionName = "CreateUsers", Description = "Can create new users"},
        new() { PermissionName = "ReadUsers", Description = "Can view all users"},
        new() { PermissionName = "DeleteUsers", Description = "Can delete users"},
        new() { PermissionName = "BlockUsers", Description = "Can block and unblock users"},
        new() { PermissionName = "CreateRoles", Description = "Can reset a password of any user"},
        new() { PermissionName = "ReadRoles", Description = "Can reset a password of any user"},
        new() { PermissionName = "UpdateRoles", Description = "Can reset a password of any user"},
        new() { PermissionName = "DeleteRoles", Description = "Can reset a password of any user"},
        new() { PermissionName = "CreatePermissions", Description = "Can reset a password of any user"},
        new() { PermissionName = "ReadPermissions", Description = "Can reset a password of any user"},
        new() { PermissionName = "DeletePermissions", Description = "Can reset a password of any user"},
        new() { PermissionName = "UpdateRolePermissions", Description = "Can reset a password of any user"},
        new() { PermissionName = "UpdateUserRoles", Description = "Can reset a password of any user"}
    };
    public static List<PermissionModel> GetPermissions()
    {
        return permissions;
    }
}