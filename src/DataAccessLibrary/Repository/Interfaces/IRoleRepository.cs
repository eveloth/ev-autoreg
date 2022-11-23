using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleModel>> GetRoles(PaginationFilter filter, CancellationToken cts);
    Task<RoleModel> AddRole(string roleName, CancellationToken cts);
    Task<RoleModel> ChangeRoleName(RoleModel role, CancellationToken cts);
    Task<RoleModel> DeleteRole(int roleId, CancellationToken cts);
    Task<bool> DoesRoleExist(int roleId, CancellationToken cts);
    Task<bool> DoesRoleExist(string roleName, CancellationToken cts);
    Task<UserProfileModel> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserProfileModel> RemoveUserFromRole(int userId, CancellationToken cts);
}