using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
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

    public async Task<IEnumerable<RoleModel>> GetAll(PaginationFilter filter, CancellationToken cts)
    {
        var take = filter.PageSize;
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var sql = @$"SELECT * FROM role ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<RoleModel>(sql, cts);
    }

    public async Task<RoleModel> Add(RoleModel role, CancellationToken cts)
    {
        const string sql = @"INSERT INTO role (role_name) VALUES (@RoleName) RETURNING *";

        var parameters = new DynamicParameters(role);

        return await _db.SaveData<RoleModel>(sql, parameters, cts);
    }

    public async Task<RoleModel> ChangeName(RoleModel role, CancellationToken cts)
    {
        //There's a point that I'll address later:
        //Here I have an @Id parameter placeholder, not @RoleId, because I'm getting it from RoleModel,
        //and not from anonymous object. I think this should me somehow unified,
        //otherwise we have a little bit of obscurity.
        //If you attempt to change it for 'readability' or w/ever, it will break PUT /api/access-control/roles/{id}.
        const string sql = @"UPDATE role SET role_name = @RoleName WHERE id = @Id RETURNING *";

        var parameters = new DynamicParameters(role);

        return await _db.SaveData<RoleModel>(sql, parameters, cts);
    }

    public async Task<RoleModel> Delete(int roleId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM role WHERE id = @RoleId RETURNING *";

        var parameters = new DynamicParameters(new { RoleId = roleId });

        return await _db.SaveData<RoleModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(int roleId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE id = @RoleId)";

        var parameters = new DynamicParameters(new { RoleId = roleId });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string roleName, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE role_name = @RoleName)";

        var parameters = new DynamicParameters(new { RoleName = roleName });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }

    public async Task<UserModel> SetUserRole(int userId, int roleId, CancellationToken cts)
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

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserModel> RemoveUserFromRole(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated AS
                            (UPDATE app_user SET role_id = null 
                             WHERE id = @UserId
                             RETURNING *)
                             SELECT * FROM updated
                             LEFT JOIN role
                             ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }
}