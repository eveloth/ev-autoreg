using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Filters;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermissionRecord>> GetAllRolePermissions(PaginationFilter filter, CancellationToken cts);
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
    Task<bool> DoesRolePermissionCorrecationExist(
        int roleId,
        int permissionId,
        CancellationToken cts
    );
}
