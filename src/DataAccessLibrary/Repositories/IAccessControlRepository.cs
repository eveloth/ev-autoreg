using DataAccessLibrary.DisplayModels;

namespace DataAccessLibrary.Repositories;

public interface IAccessControlRepository
{
    Task<IEnumerable<RoleModel?>> GetRoles(CancellationToken cts);
    Task<RoleModel> AddRole(string roleName, CancellationToken cts);
    Task<RoleModel> ChangeRoleName(RoleModel role, CancellationToken cts);
    Task<RoleModel> DeleteRole(int roleId, CancellationToken cts);
    Task<bool> DoesRoleExist(int roleId, CancellationToken cts);
    Task<IEnumerable<PermissionModel>> GetAllPermissions(CancellationToken cts);

    Task<PermissionModel> AddPermission(
        PermissionModel permission,
        CancellationToken cts
    );

    Task<PermissionModel> DeletePermission(int permissionId, CancellationToken cts);
    Task<int> ClearPermissions(CancellationToken cts);
    Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts);
    Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> GetAllRolePermissions(CancellationToken cts);
    Task<RolePermissionModel> GetRolePermissions(int roleId, CancellationToken cts);

    Task<RolePermissionModel> AddPermissionToRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );

    Task<RolePermissionModel> DeletePermissionFromRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );

    Task<bool> DoesRolePermissionCorrecationExist(int roleId, int permissionId, CancellationToken cts);
    Task<UserProfileModel> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserProfileModel> RemoveUserFromRole(int userId, CancellationToken cts);
}