using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Data;

public class UserRepository
{
    private readonly ISqlDataAccess _db;
    
    public UserRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task CreateUser(UserModel user)
    {
        var parameters = new DynamicParameters(user);
        const string sql = "INSERT INTO app_user (email, password_hash, first_name, last_name, is_blocked, is_deleted)" +
                           "VALUES (@Email, @PasswordHash, @FirstName, @LastName, @IsBlocked, @IsDeleted)";

        await _db.SaveData(sql, parameters);
    }

    public async Task<UserModel?> GetUser(int id)
    {
        const string sql = "SELECT * FROM app_user WHERE id = @Id and is_deleted = false";

        var result = await _db.LoadData<UserModel, int>(sql, id);
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<UserModel>> GetAllUsers()
    {
        const string sql = "SELECT * FROM app_user WHERE is_deleted = false";

        return await _db.LoadAllData<UserModel>(sql);
    }
    
    public async Task<IEnumerable<UserModel>> GetAllUsersIncludingDeleted()
    {
        const string sql = "SELECT * FROM app_user";

        return await _db.LoadAllData<UserModel>(sql);
    }

    public async Task UpdateUser(UserModel user)
    {
        var parameters = new DynamicParameters(user);
        const string sql = "UPDATE app_user SET email = @Email, password_hash = @PasswordHash," +
                           "first_name = @Firstname, last_name = @lastName, is_blocked = @IsBlocked," +
                           "is_deleted = @IsDeleted WHERE id = @Id";

        await _db.SaveData(sql, parameters);
    }

    public async Task DeleteUser(int id)
    {
        const string sql = "UPDATE app_user SET is_deleted = true WHERE id = @Id";

        await _db.SaveData(sql, id);
    }
    
    public async Task BlockUser(int id)
    {
        const string sql = "UPDATE app_user SET is_blocked = true WHERE id = @Id";

        await _db.SaveData(sql, id);
    }
}