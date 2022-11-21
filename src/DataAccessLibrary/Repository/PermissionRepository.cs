using Dapper;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class PermissionRepository : IPermissionRepository
{
    private readonly ISqlDataAccess _db;

    public PermissionRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Permission>> GetAllPermissions(PaginationFilter filter, CancellationToken cts)
    {
        var take = filter.Pagesize;
        var skip = (filter.PageNumber - 1) * filter.Pagesize;
        
        var sql = @$"SELECT * FROM permission ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<Permission>(sql, cts);
    }

    public async Task<Permission> AddPermission(PermissionModel permission, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO permission (permission_name, description) 
              VALUES (@PermissionName, @Description)
              RETURNING *";

        var parameters = new DynamicParameters(permission);

        return await _db.SaveData<Permission>(sql, parameters, cts);
    }

    public async Task<Permission> DeletePermission(int permissionId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM permission WHERE id = @PermissionId RETURNING *";

        var parameters = new DynamicParameters(new {PermissionId = permissionId});
        
        return await _db.SaveData<Permission>(sql, parameters, cts);
    }

    public async Task<int> ClearPermissions(CancellationToken cts)
    {
        const string sql =
            @"WITH deleted AS
                             (DELETE FROM permission
                             RETURNING 1)
                             SELECT COUNT(*) FROM deleted";

        return await _db.SaveData<int>(sql, cts);
    }

    public async Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM permission WHERE id = @PermissionId)";

        var parameters = new DynamicParameters(new {PermissionId = permissionId});
        
        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM permission WHERE permission_name = @PermissionName)";

        var parameters = new DynamicParameters(new {PermissionName = permissionName});
        
        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}