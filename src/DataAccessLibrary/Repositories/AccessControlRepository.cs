using Dapper;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repositories;

public class AccessControlRepository : IAccessControlRepository
{
    private readonly ISqlDataAccess _db;

    public AccessControlRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RoleModel?>> GetRoles(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM role";

        return await _db.LoadAllData<RoleModel?>(sql, cts);
    }

    public async Task<RoleModel> AddRole(string roleName, CancellationToken cts)
    {
        const string sql = @"INSERT INTO role (role_name) VALUES (@RoleName) RETURNING *";

        return await _db.SaveData<object, RoleModel>(sql, new { RoleName = roleName }, cts);
    }

    public async Task<RoleModel> ChangeRoleName(RoleModel role, CancellationToken cts)
    {
        const string sql = @"UPDATE role SET role_name = @RoleName WHERE id = @Id RETURNING *";

        var parameters = new DynamicParameters(role);

        return await _db.SaveData<object, RoleModel>(sql, parameters, cts);
    }

    public async Task<RoleModel> DeleteRole(int roleId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM role WHERE id = @Id RETURNING *";

        return await _db.SaveData<object, RoleModel>(sql, new { Id = roleId }, cts);
    }

    public async Task<bool> DoesRoleExist(int roleId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role WHERE id = @RoleId)";

        return await _db.LoadFirst<bool, object>(sql, new { RoleId = roleId }, cts);
    }

    public async Task<IEnumerable<PermissionModel>> GetAllPermissions(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM permission";

        return await _db.LoadAllData<PermissionModel>(sql, cts);
    }
    
    public async Task<PermissionModel> AddPermission(
        PermissionModel permission,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO permission (permission_name, description) 
              VALUES (@PermissionName, @Description)
              RETURNING *";

        var parameters = new DynamicParameters(permission);

        return await _db.SaveData<object, PermissionModel>(
            sql,
            parameters,
            cts
        );
    }

    public async Task<PermissionModel> DeletePermission(int permissionId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM permission WHERE id = @Id RETURNING *";

        return await _db.SaveData<object, PermissionModel>(sql, new { Id = permissionId }, cts);
    }

    public async Task<int> ClearPermissions(CancellationToken cts)
    {
        const string sql = @"WITH deleted AS
                             (DELETE FROM permission
                             RETURNING 1)
                             SELECT COUNT(*) FROM deleted";

        return await _db.SaveData<int>(sql, cts);
    }

    public async Task<bool> DoesPermissionExist(int permissionId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM permission WHERE id = @PermissionId)";

        return await _db.LoadFirst<bool, object>(sql, new { PermissionId = permissionId }, cts);
    }

    public async Task<bool> DoesPermissionExist(string permissionName, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM permission WHERE permission_name = @PermissionName)";

        return await _db.LoadFirst<bool, object>(sql, new { PermissionName = permissionName }, cts);
    }

    public async Task<IEnumerable<RolePermissionModel>> GetAllRolePermissions(CancellationToken cts)
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

    public async Task<RolePermissionModel> GetRolePermissions(int roleId, CancellationToken cts)
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

    public async Task<RolePermissionModel> AddPermissionToRole(
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

        return await GetRolePermissions(roleId, cts);
    }

    public async Task<RolePermissionModel> DeletePermissionFromRole(
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

    public async Task<bool> DoesRolePermissionCorrecationExist(int roleId, int permissionId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM role_permission WHERE role_id = @RoleId AND permission_id = @PermissionId)";

        return await _db.LoadFirst<bool, object>(sql, new { RoleId = roleId, Permissionid = permissionId }, cts);
    }

    public async Task<UserProfileModel> SetUserRole(int userId, int roleId, CancellationToken cts)
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

        return await _db.SaveData<object, UserProfileModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserProfileModel> RemoveUserFromRole(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated AS
                            (UPDATE app_user SET role_id = null 
                             WHERE id = @UserId
                             RETURNING *)
                             SELECT * FROM updated
                             LEFT JOIN role
                             ON updated.role_id = role.id";

        return await _db.SaveData<object, UserProfileModel, RoleModel>(
            sql,
            new { UserId = userId },
            cts
        );
    }

    private static RolePermissionModel ConvertToRolePermissionModel(
        List<RolePermissionRecordModel> rolePermissionList
    )
    {
        var result = new RolePermissionModel
        {
            Role = new RoleModel
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
                new PermissionModel
                {
                    Id = record.PermissionId,
                    PermissionName = record.PermissionName,
                    Description = record.Description
                }
            );
        }

        return result;
    }
}
