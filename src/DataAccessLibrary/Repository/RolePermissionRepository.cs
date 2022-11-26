using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ISqlDataAccess _db;

    public RolePermissionRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RolePermissionModel>> GetAllRolePermissions(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.PageSize;
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var sql =
            $@"SELECT r.role_id, r.role_name,
                        p.id AS permission_id, 
                        p.permission_name AS permission_name,
                        p.description
                        FROM
                        (SELECT id AS role_id,
                        role_name AS role_name
                        FROM role
                        ORDER BY id
                        LIMIT {take} OFFSET {skip}) AS r
                        LEFT JOIN role_permission rp ON r.role_id = rp.role_id 
                        LEFT JOIN permission p ON rp.permission_id = p.id 
                        ORDER BY r.role_id, p.id";

        return await _db.LoadAllData<RolePermissionModel>(sql, cts);
    }

    public async Task<IEnumerable<RolePermissionModel>> GetRolePermissions(
        int roleId,
        CancellationToken cts
    )
    {
        const string sql =
            @"SELECT role.id AS role_id, 
                             role.role_name AS role_name, 
                             p.id AS permission_id, 
                             p.permission_name AS permission_name,
                             p.description
                             FROM role 
                             LEFT JOIN role_permission rp ON role.id = rp.role_id
                             LEFT JOIN permission p ON rp.permission_id = p.id
                             WHERE role.id = @RoleId";

        var parameters = new DynamicParameters(new { RoleId = roleId });

        return await _db.LoadData<RolePermissionModel>(sql, parameters, cts);
    }

    public async Task<IEnumerable<RolePermissionModel>> AddPermissionToRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO role_permission (role_id, permission_id) VALUES (@RoleId, @PermissionId) RETURNING role_id";

        var parameters = new DynamicParameters(
            new { RoleId = roleId, PermissionId = permissionId }
        );

        roleId = await _db.SaveData<int>(sql, parameters, cts);

        var result = await GetRolePermissions(roleId, cts);

        return result;
    }

    public async Task<IEnumerable<RolePermissionModel>> DeletePermissionFromRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    )
    {
        const string sql =
            @"DELETE FROM role_permission
                             WHERE role_id = @RoleId and permission_id = @PermissionId RETURNING role_id";

        var parameters = new DynamicParameters(
            new { RoleId = roleId, PermissionId = permissionId }
        );

        roleId = await _db.SaveData<int>(sql, parameters, cts);

        return await GetRolePermissions(roleId, cts);
    }

    public async Task<bool> DoesRolePermissionCorrecationExist(
        int roleId,
        int permissionId,
        CancellationToken cts
    )
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM role_permission WHERE role_id = @RoleId AND permission_id = @PermissionId)";

        var parameters = new DynamicParameters(
            new { RoleId = roleId, PermissionId = permissionId }
        );

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}
