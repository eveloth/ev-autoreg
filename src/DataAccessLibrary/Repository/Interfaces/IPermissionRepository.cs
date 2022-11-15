using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllPermissions(CancellationToken cts);
    Task<Permission> AddPermission(PermissionModel permission, CancellationToken cts);
    Task<Permission> DeletePermission(int permissionId, CancellationToken cts);
    Task<int> ClearPermissions(CancellationToken cts);
    Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts);
    Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts);
}
