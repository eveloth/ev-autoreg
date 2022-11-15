using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IAccessControlRepository
{
    Task<IEnumerable<Role?>> GetRoles(CancellationToken cts);
    Task<Role> AddRole(string roleName, CancellationToken cts);
    Task<Role> ChangeRoleName(RoleModel role, CancellationToken cts);
    Task<Role> DeleteRole(int roleId, CancellationToken cts);
    Task<bool> DoesRoleExist(int roleId, CancellationToken cts);
    Task<IEnumerable<Permission>> GetAllPermissions(CancellationToken cts);

    Task<Permission> AddPermission(
        PermissionModel permission,
        CancellationToken cts
    );

    Task<Permission> DeletePermission(int permissionId, CancellationToken cts);
    Task<int> ClearPermissions(CancellationToken cts);
    Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts);
    Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts);
    Task<IEnumerable<RolePermissionRecord>> GetAllRolePermissions(CancellationToken cts);
    Task<RolePermissionRecord> GetRolePermissions(int roleId, CancellationToken cts);

    Task<RolePermissionRecord> AddPermissionToRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );

    Task<RolePermissionRecord> DeletePermissionFromRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );

    Task<bool> DoesRolePermissionCorrecationExist(int roleId, int permissionId, CancellationToken cts);
    Task<UserProfile> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserProfile> RemoveUserFromRole(int userId, CancellationToken cts);
}