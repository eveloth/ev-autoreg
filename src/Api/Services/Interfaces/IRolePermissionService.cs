using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IRolePermissionService
{
    Task<IEnumerable<RolePermission>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    );
    Task<RolePermission> Get(int id, CancellationToken cts);
    Task<RolePermission> AddPermissionToRole(RolePermission rolePermission, CancellationToken cts);
    Task<RolePermission> RemovePermissionFromRole(
        RolePermission rolePermission,
        CancellationToken cts
    );
}