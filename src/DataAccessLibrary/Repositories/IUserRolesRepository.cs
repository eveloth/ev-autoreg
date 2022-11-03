using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRolesRepository
{
    Task<bool> DoesRoleExist(int roleId, CancellationToken cts);
    Task<bool> DoesRecordExist(int userId, int roleId, CancellationToken cts);
    Task<IEnumerable<RoleModel?>> GetRoles(CancellationToken cts);
    Task<RoleModel> AddRole(string roleName, CancellationToken cts);
    Task<RoleModel> ChangeRoleName(int roleId, string newRoleName, CancellationToken cts);
    Task<RoleModel> DeleteRole(int roleId, CancellationToken cts);
    Task<IEnumerable<UserRoleDisplayModel?>> GetAllUserRoles(CancellationToken cts);
    Task<UserRoleDisplayModel?> GetUserRole(int id, CancellationToken cts);
    Task<UserRoleRecordModel> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserRoleRecordModel> DeleteUserFromRole(int userId, int roleId, CancellationToken cts);
}