using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermissionModel>> GetAllRolePermissions(PaginationFilter filter, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> GetRolePermissions(int roleId, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> AddPermissionToRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );
    Task<IEnumerable<RolePermissionModel>> DeletePermissionFromRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );
    Task<bool> DoesRolePermissionCorrecationExist(
        int roleId,
        int permissionId,
        CancellationToken cts
    );
}
