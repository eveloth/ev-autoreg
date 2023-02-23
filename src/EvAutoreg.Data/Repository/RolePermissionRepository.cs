using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ISqlDataAccess _db;

    public RolePermissionRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RolePermissionModel>> GetAll(
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
                        p.description,
                        p.is_priveleged_permission
                        FROM
                        (SELECT id AS role_id,
                        role_name AS role_name,
                        is_priveleged_role
                        FROM role
                        ORDER BY id
                        LIMIT {take} OFFSET {skip}) AS r
                        LEFT JOIN role_permission rp ON r.role_id = rp.role_id 
                        LEFT JOIN permission p ON rp.permission_id = p.id 
                        ORDER BY r.role_id, p.id";

        return await _db.LoadAllData<RolePermissionModel>(sql, cts);
    }

    public async Task<IEnumerable<RolePermissionModel>> Get(int roleId, CancellationToken cts)
    {
        const string sql =
            @"SELECT role.id AS role_id, 
                             role.role_name AS role_name, 
                             is_priveleged_role,
                             p.id AS permission_id, 
                             p.permission_name AS permission_name,
                             p.description,
                             p.is_priveleged_permission
                             FROM role 
                             LEFT JOIN role_permission rp ON role.id = rp.role_id
                             LEFT JOIN permission p ON rp.permission_id = p.id
                             WHERE role.id = @RoleId";

        var parameters = new DynamicParameters(new { RoleId = roleId });

        return await _db.LoadData<RolePermissionModel>(sql, parameters, cts);
    }

    public async Task<IEnumerable<RolePermissionModel>> AddPermissionToRole(
        RolePermissionModel rolePermission,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO role_permission (role_id, permission_id) VALUES (@RoleId, @PermissionId) RETURNING role_id";

        var parameters = new DynamicParameters(rolePermission);

        var roleId = await _db.SaveData<int>(sql, parameters, cts);

        var result = await Get(roleId, cts);

        return result;
    }

    public async Task<IEnumerable<RolePermissionModel>> RemovePermissionFromRole(
        RolePermissionModel rolePermission,
        CancellationToken cts
    )
    {
        const string sql =
            @"DELETE FROM role_permission
                             WHERE role_id = @RoleId and permission_id = @PermissionId RETURNING role_id";

        var parameters = new DynamicParameters(rolePermission);

        var roleId = await _db.SaveData<int>(sql, parameters, cts);

        return await Get(roleId, cts);
    }

    public async Task<bool> DoesCorrelationExist(
        RolePermissionModel rolePermission,
        CancellationToken cts
    )
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM role_permission WHERE role_id = @RoleId AND permission_id = @PermissionId)";

        var parameters = new DynamicParameters(rolePermission);

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }
}