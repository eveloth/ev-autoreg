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

    public async Task<bool> DoesRoleExist(int roleId)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE id = @RoleId)";

        return await _db.LoadFirst<bool, object>(sql, new { RoleId = roleId });
    }

    public async Task<bool> DoesRecordExist(int userId, int roleId)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM user_roles WHERE user_id = @UserId and role_id = @RoleId)";
        
        var parameters = new DynamicParameters(new {UserId = userId, RoleId = roleId });

        return await _db.LoadFirst<bool, object>(sql, parameters);
    }

    public async Task<IEnumerable<RoleModel?>> GetRoles()
    {
        const string sql = @"SELECT * FROM role";

        return await _db.LoadAllData<RoleModel?>(sql);
    }
    
    public async Task AddRole(string roleName)
    {
        const string sql = @"INSERT INTO role (role_name) VALUES (@RoleName)";

        await _db.SaveData(sql, new {RoleName = roleName});
    }


    public async Task ChangeRoleName(int roleId, string newRoleName)
    {
        const string sql = @"UPDATE role SET role_name = @RoleName WHERE id = @Id";
        
        var parameters = new DynamicParameters(new {RoleName = newRoleName, Id = roleId});
        
        await _db.SaveData(sql, parameters);
    }

    public async Task DeleteRole(int roleId)
    {
        const string sql = @"DELETE FROM role WHERE id = @Id";

        await _db.SaveData(sql, new {Id = roleId});
    }
    
    public async Task<IEnumerable<UserRoleModel?>> GetAllUserRoles()
    {
        const string sql = @"SELECT app_user.id AS user_id, email, role_name
                             FROM app_user
                             INNER JOIN user_roles
                             ON app_user.id = user_roles.user_id
                             INNER JOIN role
                             ON user_roles.role_id = role.id";

        return await _db.LoadAllData<UserRoleModel?>(sql);
    }
    
    public async Task<UserRoleModel?> GetUserRole(int id)
    {
        const string sql = @"SELECT app_user.id AS user_id, email, role_name
                             FROM app_user
                             INNER JOIN user_roles
                             ON app_user.id = user_roles.user_id
                             INNER JOIN role
                             ON user_roles.role_id = role.id
                             WHERE app_user.id = @Id";

        return await _db.LoadFirst<UserRoleModel?, object>(sql, new {Id = id});
    }

    public async Task<bool> SetUserRole(int userId, int roleId)
    {
        const string addUserToRoleQuery = @"INSERT INTO user_roles (user_id, role_id)
                                            VALUES (@UserId, @RoleId)
                                            ON CONFLICT (user_id)
                                            DO UPDATE SET
                                            role_id = EXCLUDED.role_id";

        var parameters = new DynamicParameters(new {UserId = userId, RoleId = roleId });

        try
        {
            await _db.SaveData(addUserToRoleQuery, parameters);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Domething went wrong.");
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteUserFromRole(int userId, int roleId)
    {
        const string existingRecordQuery = 
            @"SELECT EXISTS (SELECT true FROM user_roles WHERE user_id = @UserId and role_id = @RoleId)";
        
        var parameters = new DynamicParameters(new {UserId = userId, RoleId = roleId });

        var recordExists = await _db.LoadFirst<bool, object>(existingRecordQuery, parameters);

        if (!recordExists) return false;

        const string deleteFromRoleQuery = @"DELETE FROM user_roles WHERE user_id = @UserId and role_id = @RoleId";

        await _db.SaveData(deleteFromRoleQuery, parameters);

        return true;
    }
}
