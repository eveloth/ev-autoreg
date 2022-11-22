using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<PermissionModel>> GetAllPermissions(PaginationFilter filter, CancellationToken cts);
    Task<PermissionModel> AddPermission(PermissionModel permission, CancellationToken cts);
    Task<PermissionModel> DeletePermission(int permissionId, CancellationToken cts);
    Task<int> ClearPermissions(CancellationToken cts);
    Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts);
    Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts);
}
