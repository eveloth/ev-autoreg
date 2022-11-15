using Dapper;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
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

    public async Task<IEnumerable<RolePermissionRecord>> GetAllRolePermissions(
        CancellationToken cts
    )
    {
        const string sql =
            @"SELECT role.id AS role_id, 
                             role.role_name AS role_name, 
                             p.id AS permission_id, 
                             p.permission_name AS permission_name
                             FROM role 
                             LEFT JOIN role_permission rp ON role.id = rp.role_id 
                             LEFT JOIN permission p ON rp.permission_id = p.id";

        var rolePermissionSet = await _db.LoadAllData<RolePermissionRecordModel>(sql, cts);

        var rolePermissionList = rolePermissionSet.ToList();

        var permissionGroups = rolePermissionList.GroupBy(x => new { x.RoleId, x.RoleName });

        return permissionGroups
            .Select(group => group.Select(x => x).ToList())
            .Select(ConvertToRolePermissionModel)
            .ToList();
    }

    public async Task<RolePermissionRecord> GetRolePermissions(int roleId, CancellationToken cts)
    {
        const string sql =
            @"SELECT role.id AS role_id, 
                             role.role_name AS role_name, 
                             p.id AS permission_id, 
                             p.permission_name AS permission_name
                             FROM role 
                             LEFT JOIN role_permission rp ON role.id = rp.role_id
                             LEFT JOIN permission p ON rp.permission_id = p.id
                             WHERE role.id = @RoleId";

        var rolePermissionSet = await _db.LoadData<RolePermissionRecordModel, object>(
            sql,
            new { RoleId = roleId },
            cts
        );

        var rolePermissionList = rolePermissionSet.ToList();

        var result = ConvertToRolePermissionModel(rolePermissionList);

        return result;
    }

    public async Task<RolePermissionRecord> AddPermissionToRole(
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

        roleId = await _db.SaveData<object, int>(sql, parameters, cts);

        var result = await GetRolePermissions(roleId, cts);

        return result;
    }

    public async Task<RolePermissionRecord> DeletePermissionFromRole(
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

        roleId = await _db.SaveData<object, int>(sql, parameters, cts);

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

        return await _db.LoadFirst<bool, object>(
            sql,
            new { RoleId = roleId, Permissionid = permissionId },
            cts
        );
    }

    private static RolePermissionRecord ConvertToRolePermissionModel(
        List<RolePermissionRecordModel> rolePermissionList
    )
    {
        var result = new RolePermissionRecord
        {
            Role = new Role
            {
                Id = rolePermissionList.First().RoleId,
                RoleName = rolePermissionList.First().RoleName
            }
        };

        if (rolePermissionList.First().PermissionId is null)
            return result;

        foreach (var record in rolePermissionList)
        {
            result.Permissions.Add(
                new Permission
                {
                    Id = record.PermissionId!.Value,
                    PermissionName = record.PermissionName,
                    Description = record.Description
                }
            );
        }

        return result;
    }
}
