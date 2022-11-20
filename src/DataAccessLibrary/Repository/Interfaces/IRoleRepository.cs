using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Filters;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role?>> GetRoles(PaginationFilter filter, CancellationToken cts);
    Task<Role> AddRole(string roleName, CancellationToken cts);
    Task<Role> ChangeRoleName(RoleModel role, CancellationToken cts);
    Task<Role> DeleteRole(int roleId, CancellationToken cts);
    Task<bool> DoesRoleExist(int roleId, CancellationToken cts);
    Task<UserProfile> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserProfile> RemoveUserFromRole(int userId, CancellationToken cts);
}
