using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermissionModel>> GetAll(CancellationToken cts, PaginationFilter? filter = null);
    Task<IEnumerable<RolePermissionModel>> Get(int roleId, CancellationToken cts);
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