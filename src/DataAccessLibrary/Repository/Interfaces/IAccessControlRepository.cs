using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;

namespace DataAccessLibrary.Repository;

public interface IAccessControlRepository
{
    Task<IEnumerable<RoleModel?>> GetRoles(CancellationToken cts);
    Task<RoleModel> AddRole(string roleName, CancellationToken cts);
    Task<RoleModel> ChangeRoleName(RoleModel role, CancellationToken cts);
    Task<RoleModel> DeleteRole(int roleId, CancellationToken cts);
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
    Task<UserProfileModel> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserProfileModel> RemoveUserFromRole(int userId, CancellationToken cts);
}