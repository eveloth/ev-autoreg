using Dapper;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly ISqlDataAccess _db;

    public RoleRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Role?>> GetRoles(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM role";

        return await _db.LoadAllData<Role?>(sql, cts);
    }

    public async Task<Role> AddRole(string roleName, CancellationToken cts)
    {
        const string sql = @"INSERT INTO role (role_name) VALUES (@RoleName) RETURNING *";

        return await _db.SaveData<object, Role>(sql, new { RoleName = roleName }, cts);
    }

    public async Task<Role> ChangeRoleName(RoleModel role, CancellationToken cts)
    {
        const string sql = @"UPDATE role SET role_name = @RoleName WHERE id = @Id RETURNING *";

        var parameters = new DynamicParameters(role);

        return await _db.SaveData<object, Role>(sql, parameters, cts);
    }

    public async Task<Role> DeleteRole(int roleId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM role WHERE id = @Id RETURNING *";

        return await _db.SaveData<object, Role>(sql, new { Id = roleId }, cts);
    }

    public async Task<bool> DoesRoleExist(int roleId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE id = @RoleId)";

        return await _db.LoadFirst<bool, object>(sql, new { RoleId = roleId }, cts);
    }

    public async Task<UserProfile> SetUserRole(int userId, int roleId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated AS
                            (UPDATE app_user 
                             SET role_id = @RoleId WHERE id = @UserId 
                             RETURNING *)
                             SELECT * FROM updated
                             LEFT JOIN role
                             ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId, RoleId = roleId });

        return await _db.SaveData<object, UserProfile, Role>(sql, parameters, cts);
    }

    public async Task<UserProfile> RemoveUserFromRole(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated AS
                            (UPDATE app_user SET role_id = null 
                             WHERE id = @UserId
                             RETURNING *)
                             SELECT * FROM updated
                             LEFT JOIN role
                             ON updated.role_id = role.id";

        return await _db.SaveData<object, UserProfile, Role>(sql, new { UserId = userId }, cts);
    }
}
