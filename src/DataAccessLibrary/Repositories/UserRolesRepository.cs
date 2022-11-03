using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repositories;

public class UserRolesRepository : IUserRolesRepository
{
    private readonly ISqlDataAccess _db;

    public UserRolesRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<bool> DoesRoleExist(int roleId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE id = @RoleId)";

        return await _db.LoadFirst<bool, object>(sql, new { RoleId = roleId }, cts);
    }

    public async Task<bool> DoesRecordExist(int userId, int roleId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM user_roles WHERE user_id = @UserId and role_id = @RoleId)";
        
        var parameters = new DynamicParameters(new {UserId = userId, RoleId = roleId });

        return await _db.LoadFirst<bool, object>(sql, parameters, cts);
    }

    public async Task<IEnumerable<RoleModel?>> GetRoles(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM role";

        return await _db.LoadAllData<RoleModel?>(sql, cts);
    }
    
    public async Task<RoleModel> AddRole(string roleName, CancellationToken cts)
    {
        const string sql = @"INSERT INTO role (role_name) VALUES (@RoleName) RETURNING *";

        return await _db.SaveData<object, RoleModel>(sql, new {RoleName = roleName}, cts);
    }


    public async Task<RoleModel> ChangeRoleName(int roleId, string newRoleName, CancellationToken cts)
    {
        const string sql = @"UPDATE role SET role_name = @RoleName WHERE id = @Id RETURNING *";
        
        var parameters = new DynamicParameters(new {RoleName = newRoleName, Id = roleId});
        
        return await _db.SaveData<object, RoleModel>(sql, parameters, cts);
    }

    public async Task<RoleModel> DeleteRole(int roleId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM role WHERE id = @Id RETURNING *";

        return await _db.SaveData<object, RoleModel>(sql, new {Id = roleId}, cts);
    }
    
    public async Task<IEnumerable<UserRoleDisplayModel?>> GetAllUserRoles(CancellationToken cts)
    {
        const string sql = @"SELECT app_user.id AS user_id, email, role_id, role_name
                             FROM app_user
                             INNER JOIN user_roles
                             ON app_user.id = user_roles.user_id
                             INNER JOIN role
                             ON user_roles.role_id = role.id";

        return await _db.LoadAllData<UserRoleDisplayModel?>(sql, cts);
    }
    
    public async Task<UserRoleDisplayModel?> GetUserRole(int id, CancellationToken cts)
    {
        const string sql = @"SELECT app_user.id AS user_id, email, role_id, role_name
                             FROM app_user
                             INNER JOIN user_roles
                             ON app_user.id = user_roles.user_id
                             INNER JOIN role
                             ON user_roles.role_id = role.id
                             WHERE app_user.id = @Id";

        return await _db.LoadFirst<UserRoleDisplayModel?, object>(sql, new {Id = id}, cts);
    }

    public async Task<UserRoleRecordModel> SetUserRole(int userId, int roleId, CancellationToken cts)
    {
        const string addUserToRoleQuery = @"INSERT INTO user_roles (user_id, role_id)
                                            VALUES (@UserId, @RoleId)
                                            ON CONFLICT (user_id)
                                            DO UPDATE SET
                                            role_id = EXCLUDED.role_id RETURNING user_id, role_id";

        var parameters = new DynamicParameters(new {UserId = userId, RoleId = roleId });

        return await _db.SaveData<object, UserRoleRecordModel>(addUserToRoleQuery, parameters, cts);
    }

    public async Task<UserRoleRecordModel> DeleteUserFromRole(int userId, int roleId, CancellationToken cts)
    {
        var parameters = new DynamicParameters(new {UserId = userId, RoleId = roleId });

        const string deleteFromRoleQuery = @"DELETE FROM user_roles WHERE user_id = @UserId and role_id = @RoleId RETURNING user_id, role_id";

        return await _db.SaveData<object, UserRoleRecordModel>(deleteFromRoleQuery, parameters, cts);
    }
}
