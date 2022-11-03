using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRepository
{
    Task<bool> DoesUserExist(int id, CancellationToken cts);
    Task<bool> DoesUserExist(string email, CancellationToken cts);
    Task<UserProfile> CreateUser(UserModel user, CancellationToken cts);
    Task<UserModel?> GetUserById(int id, CancellationToken cts, bool includeDeleted = false);
    Task<UserModel?> GetUserByEmail(string email, CancellationToken cts, bool includeDeleted = false);
    Task<UserProfile?> GetUserProfle(int id, CancellationToken cts, bool includeDeleted = false);
    Task<IEnumerable<UserProfile>> GetAllUserProfiles(CancellationToken cts, bool includeDeleted = false);
    Task<int> UpdateUserPassword(int id, string passwordHash, CancellationToken cts);
    Task<UserProfile> UpdateUserEmail(int id, string newEmail, CancellationToken cts);
    Task<UserProfile> UpdateUserProfile(int id, string firstName, string lastName, CancellationToken cts);
    Task<int> BlockUser(int id, CancellationToken cts);
    Task<int> UnblockUser(int id, CancellationToken cts);
    Task<UserProfile> DeleteUser(int id, CancellationToken cts);
}