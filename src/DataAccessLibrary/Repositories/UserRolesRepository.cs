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

    public async Task<bool> DoesRoleExist(string roleName)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE role_name = @RoleName)";

        return await _db.LoadFirst<bool, object>(sql, new {RoleName = roleName});
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

    public async Task<IEnumerable<UserRoleModel?>> GetUserRoles()
    {
        const string sql = @"SELECT app_user.id AS user_id, email, role_name
                             FROM app_user
                             INNER JOIN user_roles
                             ON app_user.id = user_roles.user_id
                             INNER JOIN role
                             ON user_roles.role_id = role.id";

        return await _db.LoadAllData<UserRoleModel?>(sql);
    }

    public async Task<bool> SetUserRole(int userId, string roleName)
    {
        const string roleIdQuery = @"SELECT id FROM role WHERE role_name = @RoleName";

        var isSuccessful = false;

        var roleId = await _db.LoadFirst<int, object>(roleIdQuery, new { RoleName = roleName} );

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
}
