using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _db;

    public UserRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<bool> DoesUserExist(int id, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE id = @Id)";

        return await _db.LoadFirst<bool, object>(sql, new { Id = id }, cts);
    }

    public async Task<bool> DoesUserExist(string email, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE email = @Email)";

        return await _db.LoadFirst<bool, object>(sql, new { email }, cts);
    }

    public async Task<IEnumerable<UserModel>> GetAllUsers(
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var sql = includeDeleted switch
        {
            true => @"SELECT * FROM app_user",
            false => @"SELECT * FROM app_user WHERE is_deleted = false"
        };

        return await _db.LoadAllData<UserModel, RoleModel>(sql, cts);
    }

    public async Task<UserModel?> GetUserById(
        int id,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var sql = includeDeleted switch
        {
            true => @"SELECT * FROM app_user WHERE id = @Id",
            false => @"SELECT * FROM app_user WHERE id = @Id and is_deleted = false"
        };

        return await _db.LoadFirst<UserModel, RoleModel, object>(sql, new { Id = id }, cts);
    }

    public async Task<UserModel?> GetUserByEmail(
        string email,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var sql = includeDeleted switch
        {
            true => @"SELECT * FROM app_user WHERE email = @Email",
            false => @"SELECT * FROM app_user WHERE email = @Email and is_deleted = false"
        };

        // В отличие от ситуции, когда в качестве параметра передаётся int, просто так передать string в качестве
        // параметра у меня не вышло; operator does not exist: @ character varying
        return await _db.LoadFirst<UserModel, RoleModel, object>(sql, new { email }, cts);
    }

    public async Task<UserProfileModel> CreateUser(UserModel user, CancellationToken cts)
    {
        var parameters = new DynamicParameters(user);

        const string sql = @"WITH inserted AS
                             (INSERT INTO app_user (email, password_hash)
                             VALUES (@Email, @PasswordHash)
                             RETURNING * )
                             SELECT * FROM inserted
                             LEFT JOIN role
                             ON inserted.role_id = role.id";

        return await _db.SaveData<object, UserProfileModel, RoleModel>(sql, parameters, cts);
    }

    public async Task<IEnumerable<UserProfileModel>> GetAllUserProfiles(
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var sql = includeDeleted switch
        {
            true
                => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user",
            false
                => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user 
                     WHERE is_deleted = false"
        };

        return await _db.LoadAllData<UserProfileModel>(sql, cts);
    }

    public async Task<UserProfileModel?> GetUserProfle(
        int id,
        CancellationToken cts,
        bool includeDeleted = false
    )
    {
        var sql = includeDeleted switch
        {
            true
                => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user WHERE id = @Id",
            false
                => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user WHERE id = @Id and is_deleted = false"
        };

        return await _db.LoadFirst<UserProfileModel?, object>(sql, new { Id = id }, cts);
    }

    public async Task<int> UpdateUserPassword(int id, string passwordHash, CancellationToken cts)
    {
        var parameters = new DynamicParameters(new { Id = id, PasswordHash = passwordHash });
        const string sql =
            @"UPDATE app_user SET password_hash = @PasswordHash WHERE id = @Id RETURNING id";

        return await _db.SaveData<object, int>(sql, parameters, cts);
    }

    public async Task<UserProfileModel> UpdateUserEmail(int id, string newEmail, CancellationToken cts)
    {
        var parameters = new DynamicParameters(new { Id = id, Email = newEmail });
        const string sql =
            @"UPDATE app_user SET email = @Email WHERE id = @Id
                             RETURNING id, email, first_name, last_name, is_deleted, is_blocked";

        return await _db.SaveData<object, UserProfileModel>(sql, parameters, cts);
    }

    public async Task<UserProfileModel> UpdateUserProfile(
        int id,
        string firstName,
        string lastName,
        CancellationToken cts
    )
    {
        var parameters = new DynamicParameters(
            new { Id = id, FirstName = firstName, LastName = lastName }
        );
        const string sql =
            @"UPDATE app_user SET first_name = @FirstName, last_name = @LastName WHERE id = @Id
                             RETURNING id, email, first_name, last_name, is_deleted, is_blocked";

        return await _db.SaveData<object, UserProfileModel>(sql, parameters, cts);
    }

    public async Task<int> BlockUser(int id, CancellationToken cts)
    {
        const string sql = @"UPDATE app_user SET is_blocked = true WHERE id = @Id RETURNING id";

        return await _db.SaveData<object, int>(sql, new { Id = id }, cts);
    }

    public async Task<int> UnblockUser(int id, CancellationToken cts)
    {
        const string sql = @"UPDATE app_user SET is_blocked = false WHERE id = @Id RETURNING id";

        return await _db.SaveData<object, int>(sql, new { Id = id }, cts);
    }

    public async Task<UserProfileModel> DeleteUser(int id, CancellationToken cts)
    {
        const string sql =
            @"UPDATE app_user SET is_deleted = true WHERE id = @Id
                             RETURNING id, email, first_name, last_name, is_deleted, is_blocked";

        return await _db.SaveData<object, UserProfileModel>(sql, new { Id = id }, cts);
    }
    
    public async Task<NewUserModel?> GetNewUserModel(
        int id,
        CancellationToken cts
    )
    {
        const string sql = @"SELECT * FROM app_user
                             LEFT JOIN role
                             ON app_user.role_id = role.id
                             WHERE app_user.id = @UserId";

        return await _db.LoadFirst<NewUserModel, RoleModel, object>(sql, new {UserId = id}, cts);
    }
    
}
