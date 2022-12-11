using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermissionModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> GetRole(int roleId, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> AddPermissionToRole(
        RolePermissionModel rolePermission,
        CancellationToken cts
    );
    Task<IEnumerable<RolePermissionModel>> RemovePermissionFromRole(
        RolePermissionModel rolePermission,
        CancellationToken cts
    );
    Task<bool> DoesCorrelationExist(RolePermissionModel rolePermission, CancellationToken cts);
}