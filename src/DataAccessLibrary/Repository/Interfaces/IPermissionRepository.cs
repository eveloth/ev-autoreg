using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Filters;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllPermissions(PaginationFilter filter, CancellationToken cts);
    Task<Permission> AddPermission(PermissionModel permission, CancellationToken cts);
    Task<Permission> DeletePermission(int permissionId, CancellationToken cts);
    Task<int> ClearPermissions(CancellationToken cts);
    Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts);
    Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts);
}
