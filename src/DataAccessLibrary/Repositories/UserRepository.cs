using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;
using System.Linq;

namespace DataAccessLibrary.Repositories;

public class UserRepository : IUserRepository 
{
    private readonly ISqlDataAccess _db;
    
    public UserRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<bool> DoesUserExist(int id)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE id = @Id";

        return await _db.LoadFirst<bool, int>(sql, id);
    }

    public async Task<bool> DoesUserExist(string email)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE email = @Email)";

        return await _db.LoadFirst<bool, object>(sql, new { email });
    }
    
    public async Task CreateUser(UserModel user)
    {
        var parameters = new DynamicParameters(user);
        const string sql = @"INSERT INTO app_user (email, password_hash, first_name, last_name, is_blocked, is_deleted)" +
                           @"VALUES (@Email, @PasswordHash, @FirstName, @LastName, @IsBlocked, @IsDeleted)";

        await _db.SaveData(sql, parameters);
    }

    public async Task<UserModel?> GetUserById(int id, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true  => @"SELECT * FROM app_user WHERE id = @Id",
            false => @"SELECT * FROM app_user WHERE id = @Id and is_deleted = false"
        };

        return await _db.LoadFirst<UserModel, int>(sql, id);
    }
    
    public async Task<UserModel?> GetUserByEmail(string email, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true  => @"SELECT * FROM app_user WHERE email = @Email",
            false => @"SELECT * FROM app_user WHERE email = @Email and is_deleted = false"
        };
        
        // В отличие от ситуции, когда в качестве параметра передаётся int, просто так передать string в качестве
        // параметра у меня не вышло; operator does not exist: @ character varying
        return await _db.LoadFirst<UserModel, object>(sql, new { email });
    }

    public async Task<IEnumerable<UserModel>> GetAllUsers(bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true => @"SELECT * FROM app_user",
            false => @"SELECT * FROM app_user WHERE is_deleted = false"
        };

        return await _db.LoadAllData<UserModel>(sql);
    }

    public async Task UpdateUser(UserModel user)
    {
        var parameters = new DynamicParameters(user);
        const string sql = @"UPDATE app_user SET email = @Email, password_hash = @PasswordHash," +
                           @"first_name = @Firstname, last_name = @lastName, is_blocked = @IsBlocked," +
                           @"is_deleted = @IsDeleted WHERE id = @Id";

        await _db.SaveData(sql, parameters);
    }

    public async Task ChangeUserPassword(int id, string passwordHash)
    {
        var parameters = new DynamicParameters(new {Id = id, PasswordHash = passwordHash});
        const string sql = @"UPDATE app_user SET password_hash = @PasswordHash WHERE id = @Id";

        await _db.SaveData(sql, parameters);
    }

    public async Task UpdateUserEmail(int id, string newEmail)
    {
        var paramaters = new DynamicParameters(new {Id = id, Email = newEmail});
        const string sql = @"UPDATE app_user SET email = @Email WHERE id = @Id";

        await _db.SaveData(sql, paramaters);
    }

    public async Task UpdateUserProfile(int id, string firstName, string lastName)
    {
        var parameters = new DynamicParameters(new {Id = id, FirstName = firstName, LastName = lastName});
        const string sql = @"UPDATE app_user SET first_name = @FirstName, last_name = @LastName WHERE id = @Id";

        await _db.SaveData(sql, parameters);
    }
    
    public async Task BlockUser(int id)
    {
        const string sql = @"UPDATE app_user SET is_blocked = true WHERE id = @Id";

        await _db.SaveData(sql, id);
    }
    
    public async Task DeleteUser(int id)
    {
        const string sql = @"UPDATE app_user SET is_deleted = true WHERE id = @Id";

        await _db.SaveData(sql, id);
    }
}