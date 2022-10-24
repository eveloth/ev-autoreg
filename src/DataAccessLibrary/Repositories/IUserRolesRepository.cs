using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRolesRepository
{
    Task<bool> DoesRoleExist(int roleId);
    Task<bool> DoesRecordExist(int userId, int roleId);
    Task<IEnumerable<RoleModel?>> GetRoles();
    Task AddRole(string roleName);
    Task ChangeRoleName(int roleId, string newRoleName);
    Task DeleteRole(int roleId);
    Task<IEnumerable<UserRoleModel?>> GetAllUserRoles();
    Task<UserRoleModel?> GetUserRole(int id);
    Task<bool> SetUserRole(int userId, int roleId);
    Task<bool> DeleteUserFromRole(int userId, int roleId);
}