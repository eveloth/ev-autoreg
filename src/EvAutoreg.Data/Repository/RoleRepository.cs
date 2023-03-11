using System.Text;
using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly ISqlDataAccess _db;
    private readonly IFilterQueryBuilder _filterQueryBuilder;

    public RoleRepository(ISqlDataAccess db, IFilterQueryBuilder filterQueryBuilder)
    {
        _db = db;
        _filterQueryBuilder = filterQueryBuilder;
    }

    public async Task<IEnumerable<RoleModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sqlBuilder = new StringBuilder("SELECT * FROM role");
        _filterQueryBuilder.ApplyPaginationFilter(sqlBuilder, filter, "id");

        return await _db.LoadAllData<RoleModel>(sqlBuilder.ToString(), cts);
    }

    public async Task<RoleModel?> Get(int id, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM role WHERE id = @RoleId";

        var parameters = new DynamicParameters(new { RoleId = id });

        return await _db.LoadSingle<RoleModel?>(sql, parameters, cts);
    }

    public async Task<RoleModel> Add(RoleModel role, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO role (role_name, is_priveleged_role) VALUES (@RoleName, @IsPrivelegedRole) RETURNING *";

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

    public async Task<RoleModel> ChangePriveleges(RoleModel role, CancellationToken cts)
    {
        const string sql =
            @"UPDATE role SET is_priveleged_role = @IsPrivelegedRole WHERE id = @id RETURNING *";

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

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string roleName, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE role_name = @RoleName)";

        var parameters = new DynamicParameters(new { RoleName = roleName });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from role";
        return await _db.LoadScalar<int>(sql, cts);
    }
}