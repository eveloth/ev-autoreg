using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRolesRepository
{
    Task<bool> DoesRoleExist(string roleName);
    Task<IEnumerable<RoleModel?>> GetRoles();
    Task AddRole(string roleName);
    Task<UserRoleModel?> GetUserRole(int id);
    Task<IEnumerable<UserRoleModel?>> GetUserRoles();
    Task<bool> SetUserRole(int userId, string roleName);
}