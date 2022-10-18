using DataAccessLibrary.Models;

namespace DataAccessLibrary.Data;

public interface IUserRepository
{
    Task CreateUser(UserModel user);
    Task<UserModel?> GetUserById(int id, bool includeDeleted = false);
    Task<UserModel?> GetUserByEmail(string Email, bool includeDeleted = false);
    Task<IEnumerable<UserModel>> GetAllUsers(bool includeDeleted = false);
    Task UpdateUser(UserModel user);
    Task DeleteUser(int id);
    Task BlockUser(int id);
}