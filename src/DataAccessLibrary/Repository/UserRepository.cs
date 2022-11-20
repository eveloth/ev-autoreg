using System.Text;
using Dapper;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Filters;
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

    public async Task<User?> GetUserById(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        const string sql = @"SELECT * FROM app_user
                    LEFT JOIN role
                    ON app_user.id = role.id
                    WHERE app_user.id = @UserId";
        
        var resultingSql = includeDeleted switch
        {
            true => sql,
            false => sql + "AND" + IncludeDeletedSql
        };

        return await _db.LoadFirst<User, Role, object>(resultingSql, new { UserId = userId }, cts);
    }

    public async Task<User?> GetUserByEmail(
        string email,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {   
        const string sql = @"SELECT * FROM app_user
                    LEFT JOIN role
                    ON app_user.id = role.id
                    WHERE email = @Email";

        var resultingSql = includeDeleted switch
        {
            true => sql,
            false => sql + " AND" + IncludeDeletedSql
        };

        return await _db.LoadFirst<User, Role, object>(resultingSql, new { Email = email }, cts);
    }

    public async Task<IEnumerable<UserProfile>> GetAllUserProfiles(
        PaginationFilter filter,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var skip = (filter.PageNumber - 1) * filter.Pagesize;
        var take = filter.Pagesize;

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
            false => sqlTemplateBuilder.Append(" ").Append(paginator).ToString()
        };

        return await _db.LoadAllData<UserProfile, Role>(resultingSql, cts);
    }

    public async Task<UserProfile?> GetUserProfle(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        const string sql = @"SELECT * FROM app_user
                         LEFT JOIN role
                         ON app_user.id = role.id
                         WHERE app_user.id = @UserId";
             
        var resultingSql = includeDeleted switch
        {
            true => sql,
            false => sql + " AND" + IncludeDeletedSql
        };
        

        return await _db.LoadFirst<UserProfile?, Role, object>(resultingSql, new { UserId = userId }, cts);
    }

    public async Task<UserProfile> CreateUser(UserModel user, CancellationToken cts)
    {
        const string sql =
            @"WITH inserted AS
                             (INSERT INTO app_user (email, password_hash)
                             VALUES (@Email, @PasswordHash)
                             RETURNING
                             id, email, first_name, last_name,
                             is_blocked, is_deleted, role_id)
                             SELECT * FROM inserted
                             LEFT JOIN role
                             ON inserted.role_id = role.id";

        var parameters = new DynamicParameters(user);

        var result = await _db.SaveData<object, UserProfile, Role>(sql, parameters, cts);

        return result;
    }

    public async Task<int> UpdateUserPassword(
        int userId,
        string passwordHash,
        CancellationToken cts
    )
    {
        const string sql =
            @"UPDATE app_user SET password_hash = @PasswordHash WHERE id = @UserId RETURNING id";

        var parameters = new DynamicParameters(
            new { UserId = userId, PasswordHash = passwordHash }
        );

        return await _db.SaveData<object, int>(sql, parameters, cts);
    }

    public async Task<UserProfile> UpdateUserEmail(
        int userId,
        string newEmail,
        CancellationToken cts
    )
    {
        const string sql =
            @"WITH updated AS 
                            (UPDATE app_user SET email = @Email 
                            WHERE id = @UserId
                            RETURNING id, email, first_name, last_name, 
                            is_deleted, is_blocked, role_id)
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(new { UserId = userId, Email = newEmail });

        return await _db.SaveData<object, UserProfile, Role>(sql, parameters, cts);
    }

    public async Task<UserProfile> UpdateUserProfile(
        int userId,
        string firstName,
        string lastName,
        CancellationToken cts
    )
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET first_name = @FirstName, last_name = @LastName 
                            WHERE id = @UserId
                            RETURNING id, email, first_name, last_name, is_deleted, is_blocked, role_id)
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        var parameters = new DynamicParameters(
            new { UserId = userId, FirstName = firstName, LastName = lastName }
        );

        return await _db.SaveData<object, UserProfile, Role>(sql, parameters, cts);
    }

    public async Task<UserProfile> BlockUser(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_blocked = true
                            WHERE id = @UserId
                            RETURNING id, email, first_name, last_name, is_deleted, is_blocked, role_id)
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        return await _db.SaveData<object, UserProfile, Role>(sql, new { UserId = userId }, cts);
    }

    public async Task<UserProfile> UnblockUser(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_blocked = true
                            WHERE id = @UserId
                            RETURNING id, email, first_name, last_name, is_deleted, is_blocked, role_id)
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        return await _db.SaveData<object, UserProfile, Role>(sql, new { UserId = userId }, cts);
    }

    public async Task<UserProfile> DeleteUser(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_deleted = true
                            WHERE id = @UserId
                            RETURNING id, email, first_name, last_name, is_deleted, is_blocked, role_id)
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        return await _db.SaveData<object, UserProfile, Role>(sql, new { UserId = userId }, cts);
    }

    public async Task<UserProfile> RestoreUser(int userId, CancellationToken cts)
    {
        const string sql =
            @"WITH updated as 
                            (UPDATE app_user 
                            SET is_deleted = false
                            WHERE id = @UserId
                            RETURNING id, email, first_name, last_name, is_deleted, is_blocked, role_id)
                            SELECT * FROM updated
                            LEFT JOIN role
                            ON updated.role_id = role.id";

        return await _db.SaveData<object, UserProfile, Role>(sql, new { UserId = userId }, cts);
    }

    public async Task<bool> DoesUserExist(int userId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE id = @UserId)";

        return await _db.LoadFirst<bool, object>(sql, new { UserId = userId }, cts);
    }

    public async Task<bool> DoesUserExist(string email, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE email = @Email)";

        return await _db.LoadFirst<bool, object>(sql, new { email }, cts);
    }
}