using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IRolePermissionService
{
    Task<IEnumerable<RolePermission>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    );
    Task<RolePermission> Get(int id, CancellationToken cts);
    Task<RolePermission> AddPermissionToRole(RolePermission rolePermission, CancellationToken cts);
    Task<RolePermission> AddPrivelegedPermissionToRole(RolePermission rolePermission, CancellationToken cts);
    Task<RolePermission> RemovePermissionFromRole(
        RolePermission rolePermission,
        CancellationToken cts
    );
    Task<RolePermission> RemovePrivelegedPermissionFromRole(
        RolePermission rolePermission,
        CancellationToken cts
    );
    Task<int> Count(CancellationToken cts);
}