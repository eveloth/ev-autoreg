using EvAutoreg.Api.Domain;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Api.Seeding;

public class Permissions
{
    private static readonly List<PermissionModel> PermissionList =
        new()
        {
            new()
            {
                PermissionName = "ResetUserPasswords",
                Description = "Can reset a password of any user",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "ReadUsers",
                Description = "Can view all users",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "UpdateUsers",
                Description = "Can update users",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "AssignPrivelegedRole",
                Description = "Can add users to priveleged roles",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "RemoveFromPrivelegedRole",
                Description = "Can remove users from priveleged roles",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "DeleteUsers",
                Description = "Can delete and restore users",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "BlockUsers",
                Description = "Can block and unblock users",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "CreateRoles",
                Description = "Can create new roles",
                IsPrivelegedPermission = false
            },
            new() { PermissionName = "ReadRoles", Description = "Can view all roles" },
            new()
            {
                PermissionName = "UpdateRoles",
                Description = "Can update roles",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "AddPrivelegedPermissionToRole",
                Description = "Can assign priveleged permissions to roles",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "RemovePrivelegedPermissionFromRole",
                Description = "Can remove priveleged permissions from roles",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "DeleteRoles",
                Description = "Can delete roles",
                IsPrivelegedPermission = true
            },
            new()
            {
                PermissionName = "ReadPermissions",
                Description = "Can view all permissions",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "UseRegistrar",
                Description = "Can use registrar service",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "ConfigureRegistrar",
                Description = "Can configure registrar service",
                IsPrivelegedPermission = false
            },
            new()
            {
                PermissionName = "ForceStopRegistrar",
                Description = "Can force stop registrar not being the user who started it",
                IsPrivelegedPermission = false
            },
        };

    public static List<PermissionModel> GetPermissions()
    {
        return PermissionList;
    }
}