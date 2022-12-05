﻿using DataAccessLibrary.Models;

namespace Api;

public static class Permissions
{
    private static readonly List<PermissionModel> PermissionList = new List<PermissionModel>()
    {
        new()
        {
            PermissionName = "ResetUserPasswords",
            Description = "Can reset a password of any user"
        },
        new() { PermissionName = "CreateUsers", Description = "Can create new users" },
        new() { PermissionName = "ReadUsers", Description = "Can view all users" },
        new() { PermissionName = "UpdateUsers", Description = "Can update users" },
        new() { PermissionName = "DeleteUsers", Description = "Can delete and restore users" },
        new() { PermissionName = "BlockUsers", Description = "Can block and unblock users" },
        new() { PermissionName = "CreateRoles", Description = "Can create new roles" },
        new() { PermissionName = "ReadRoles", Description = "Can view all roles" },
        new() { PermissionName = "UpdateRoles", Description = "Can update roles" },
        new() { PermissionName = "DeleteRoles", Description = "Can delet roles" },
        new() { PermissionName = "CreatePermissions", Description = "Can create permissions" },
        new() { PermissionName = "ReadPermissions", Description = "Can view all permissions" },
        new() { PermissionName = "DeletePermissions", Description = "Can delete permissions" },
        new() { PermissionName = "UseRegistrar", Description = "Can use registrar service" },
        new()
        {
            PermissionName = "ConfigureRegistrar",
            Description = "Can configure registrar service"
        },
    };

    public static List<PermissionModel> GetPermissions()
    {
        return PermissionList;
    }
}