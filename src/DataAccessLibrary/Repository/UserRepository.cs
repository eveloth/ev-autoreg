﻿using System.Text;
using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _db;

    private const string IncludeDeletedSql = " is_deleted = false";

    public UserRepository(ISqlDataAccess db)
    {
        _db = db;
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

        var resultingSql = includeDeleted switch
        {
            true => sql,
            false => sql + " AND" + IncludeDeletedSql
        };

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadFirst<UserModel>(resultingSql, parameters, cts);
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

        var resultingSql = includeDeleted switch
        {
            true => sql,
            false => sql + " AND" + IncludeDeletedSql
        };

        var parameters = new DynamicParameters(new { Email = email });

        return await _db.LoadFirst<UserModel, RoleModel>(resultingSql, parameters, cts);
    }

    public async Task<IEnumerable<UserModel>> GetAllUserProfiles(
        PaginationFilter filter,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var skip = (filter.PageNumber - 1) * filter.PageSize;
        var take = filter.PageSize;

        var sqlTemplateBuilder = new StringBuilder(
            @"SELECT * FROM app_user
              LEFT JOIN role ON app_user.role_id = role.id"
        );

        var paginator = $@"ORDER BY app_user.id LIMIT {take} OFFSET {skip}";

        var resultingSql = includeDeleted switch
        {
            true
                => sqlTemplateBuilder
                    .Append(" WHERE")
                    .Append(IncludeDeletedSql)
                    .Append(' ')
                    .Append(paginator)
                    .ToString(),
            false => sqlTemplateBuilder.Append(' ').Append(paginator).ToString()
        };

        return await _db.LoadAllData<UserModel, RoleModel>(resultingSql, cts);
    }

    public async Task<UserModel> Create(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"WITH inserted AS
                             (INSERT INTO app_user (email, password_hash)
                             VALUES (@Email, @PasswordHash)
                             RETURNING * )
                             LEFT JOIN role
                             ON inserted.role_id = role.id";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
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
            @"WITH updated AS 
                            (UPDATE app_user SET email = @Email 
                            WHERE id = @Id
                            RETURNING * )
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserModel> UpdateUserProfile(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET first_name = @FirstName, last_name = @LastName 
                            WHERE id = @Id
                            RETURNING * )
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(user);

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Block(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_blocked = true
                            WHERE id = @UserId
                            RETURNING * )
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Unblock(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_blocked = false
                            WHERE id = @UserId
                            RETURNING * )
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId });
        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Delete(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_deleted = true
                            WHERE id = @UserId
                            RETURNING * )
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<UserModel> Restore(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_deleted = false
                            WHERE id = @UserId
                            RETURNING * )
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.SaveData<UserModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(int userId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE id = @UserId)";

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string email, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE email = @Email)";

        var parameters = new DynamicParameters(new { Email = email });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}