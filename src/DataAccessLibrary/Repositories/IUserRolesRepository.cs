using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRolesRepository
{
    Task<bool> DoesRoleExist(int roleId, CancellationToken cts);
    Task<bool> DoesRecordExist(int userId, int roleId, CancellationToken cts);
    Task<IEnumerable<RoleModel?>> GetRoles(CancellationToken cts);
    Task AddRole(string roleName, CancellationToken cts);
    Task ChangeRoleName(int roleId, string newRoleName, CancellationToken cts);
    Task DeleteRole(int roleId, CancellationToken cts);
    Task<IEnumerable<UserRoleModel?>> GetAllUserRoles(CancellationToken cts);
    Task<UserRoleModel?> GetUserRole(int id, CancellationToken cts);
    Task<bool> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<bool> DeleteUserFromRole(int userId, int roleId, CancellationToken cts);
}