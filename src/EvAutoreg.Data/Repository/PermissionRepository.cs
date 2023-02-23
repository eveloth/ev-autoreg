using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class PermissionRepository : IPermissionRepository
{
    private readonly ISqlDataAccess _db;

    public PermissionRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PermissionModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sql = @"SELECT * FROM permission";

        if (filter is not null)
        {
            var take = filter.PageSize;
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var paginator = $" ORDER BY id LIMIT {take} offset {skip}";
            sql += paginator;
        }

        return await _db.LoadAllData<PermissionModel>(sql, cts);
    }

    public async Task<PermissionModel?> Get(int permissionId, CancellationToken cts)
    {
        const string sql = $@"SELECT * FROM permission WHERE id = @PermissionId";

        var parameters = new DynamicParameters(new { PermissionId = permissionId });

        return await _db.LoadSingle<PermissionModel?>(sql, parameters, cts);
    }

    public async Task<PermissionModel> Add(PermissionModel permission, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO permission (permission_name, description, is_priveleged_permission) 
              VALUES (@PermissionName, @Description, @IsPrivelegedPermission)
              RETURNING *";

        var parameters = new DynamicParameters(permission);

        return await _db.SaveData<PermissionModel>(sql, parameters, cts);
    }

    public async Task<PermissionModel> Delete(int permissionId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM permission WHERE id = @PermissionId RETURNING *";

        var parameters = new DynamicParameters(new { PermissionId = permissionId });

        return await _db.SaveData<PermissionModel>(sql, parameters, cts);
    }

    public async Task<int> Clear(CancellationToken cts)
    {
        const string sql =
            @"WITH deleted AS
                             (DELETE FROM permission
                             RETURNING 1)
                             SELECT COUNT(*) FROM deleted";

        return await _db.SaveData<int>(sql, cts);
    }

    public async Task<bool> DoesExist(int permissionId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM permission WHERE id = @PermissionId)";

        var parameters = new DynamicParameters(new { PermissionId = permissionId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string permissionName, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM permission WHERE permission_name = @PermissionName)";

        var parameters = new DynamicParameters(new { PermissionName = permissionName });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from permission";
        return await _db.LoadScalar<int>(sql, cts);
    }
}