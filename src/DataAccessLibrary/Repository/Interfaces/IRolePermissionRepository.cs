using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermissionModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> GetRole(int roleId, CancellationToken cts);
    Task<IEnumerable<RolePermissionModel>> AddPermissionToRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );
    Task<IEnumerable<RolePermissionModel>> RemovePermissionFromRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    );
    Task<bool> DoesCorrecationExist(int roleId, int permissionId, CancellationToken cts);
}