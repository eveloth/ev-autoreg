using System.Text;
using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _db;
    private readonly IFilterQueryBuilder _filterQueryBuilder;

    private const string IncludeDeletedSql = " is_deleted = false";

    public UserRepository(ISqlDataAccess db, IFilterQueryBuilder filterQueryBuilder)
    {
        _db = db;
        _filterQueryBuilder = filterQueryBuilder;
    }

    public async Task<UserModel?> GetById(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        const string sql =
            @"SELECT * FROM app_user
                    LEFT JOIN role
                    ON app_user.id = role.id
                    WHERE app_user.id = @UserId";

        var sqlBuilder = new StringBuilder(sql);
        _filterQueryBuilder.ApplyIncludeDeletedFilter(sqlBuilder, includeDeleted, ChainOptions.And);

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadSingle<UserModel>(sqlBuilder.ToString(), parameters, cts);
    }

    public async Task<UserModel?> GetByEmail(
        string email,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        const string sql =
            @"SELECT * FROM app_user
                    LEFT JOIN role
                    ON app_user.id = role.id
                    WHERE email = @Email";

        var sqlBuilder = new StringBuilder(sql);
        _filterQueryBuilder.ApplyIncludeDeletedFilter(sqlBuilder, includeDeleted, ChainOptions.And);

        var parameters = new DynamicParameters(new { Email = email });

        return await _db.LoadSingle<UserModel, RoleModel>(sqlBuilder.ToString(), parameters, cts);
    }

    public async Task<IEnumerable<UserModel>> GetAll(
        CancellationToken cts,
        bool includeDeleted = false,
        PaginationFilter? filter = null
    )
    {
        var sqlBuilder = new StringBuilder(
            @"SELECT * FROM app_user
              LEFT JOIN role ON app_user.role_id = role.id"
        );

        _filterQueryBuilder.ApplyIncludeDeletedFilter(
            sqlBuilder,
            includeDeleted,
            ChainOptions.Where
        );

        _filterQueryBuilder.ApplyPaginationFilter(sqlBuilder, filter, "app_user.id");

        return await _db.LoadAllData<UserModel, RoleModel>(sqlBuilder.ToString(), cts);
    }

    public async Task<UserModel> Create(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO 
              app_user (email, password_hash)
              VALUES (@Email, @PasswordHash)
              RETURNING * ";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<int> UpdatePassword(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user SET password_hash = @PasswordHash WHERE id = @Id RETURNING id";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<int>(sql, parameters, cts);
    }

    public async Task<UserModel> UpdateEmail(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user SET email = @Email 
              WHERE id = @Id
              RETURNING * ";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> UpdateUserProfile(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user 
              SET first_name = @FirstName, last_name = @LastName 
              WHERE id = @Id
              RETURNING * ";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> AddUserToRole(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user 
              SET role_id = @RoleId WHERE id = @Id 
              RETURNING * ";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> RemoveUserFromRole(int userId, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user SET role_id = null 
              WHERE id = @UserId
              RETURNING * ";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Block(int userId, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user 
              SET is_blocked = true
              WHERE id = @UserId
              RETURNING * ";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Unblock(int userId, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user 
              SET is_blocked = false
              WHERE id = @UserId
              RETURNING * ";

        var parameters = new DynamicParameters(new { UserId = userId });
        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Delete(int userId, CancellationToken cts)
    {
        const string sql =
            @" UPDATE app_user 
               SET is_deleted = true
               WHERE id = @UserId
               RETURNING * ";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Restore(int userId, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user 
              SET is_deleted = false
              WHERE id = @UserId
              RETURNING * ";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(int userId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE id = @UserId)";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string email, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE email = @Email)";

        var parameters = new DynamicParameters(new { Email = email });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from app_user";
        return await _db.LoadScalar<int>(sql, cts);
    }
}