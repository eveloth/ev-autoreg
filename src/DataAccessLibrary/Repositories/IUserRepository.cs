using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRepository
{
    Task<bool> DoesUserExist(int id, CancellationToken cts);
    Task<bool> DoesUserExist(string email, CancellationToken cts);
    Task CreateUser(UserModel user, CancellationToken cts);
    Task<UserModel?> GetUserById(int id, CancellationToken cts, bool includeDeleted = false);
    Task<UserModel?> GetUserByEmail(string email, CancellationToken cts,  bool includeDeleted = false);
    Task<IEnumerable<UserModel>> GetAllUsers(CancellationToken cts, bool includeDeleted = false);
    Task UpdateUser(UserModel user, CancellationToken cts);
    Task UpdateUserPassword(int id, string passwordHash, CancellationToken cts);
    Task UpdateUserEmail(int id, string newEmail, CancellationToken cts);
    Task UpdateUserProfile(int id, string firstName, string lastName, CancellationToken cts);
    Task BlockUser(int id, CancellationToken cts);
    Task UnblockUser(int id, CancellationToken cts);
    Task DeleteUser(int id, CancellationToken cts);
}