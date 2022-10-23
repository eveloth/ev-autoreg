using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRepository
{
    Task<bool> DoesUserExist(int id);
    Task<bool> DoesUserExist(string email);
    Task CreateUser(UserModel user);
    Task<UserModel?> GetUserById(int id, bool includeDeleted = false);
    Task<UserModel?> GetUserByEmail(string email, bool includeDeleted = false);
    Task<IEnumerable<UserModel>> GetAllUsers(bool includeDeleted = false);
    Task UpdateUser(UserModel user);
    Task UpdateUserPassword(int id, string passwordHash);
    Task UpdateUserEmail(int id, string newEmail);
    Task UpdateUserProfile(int id, string firstName, string lastName);
    Task BlockUser(int id);
    Task UnblockUser(int id);
    Task DeleteUser(int id);
}