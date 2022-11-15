using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;

namespace DataAccessLibrary.Repository;

public interface IUserRepository
{
    Task<UserModel?> GetUserById(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserModel?> GetUserByEmail(
        string email,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<IEnumerable<UserProfile>> GetAllUserProfiles(
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfile?> GetUserProfle(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfile> CreateUser(UserModel user, CancellationToken cts);

    Task<int> UpdateUserPassword(
        int userId,
        string passwordHash,
        CancellationToken cts
    );

    Task<UserProfile> UpdateUserEmail(
        int userId,
        string newEmail,
        CancellationToken cts
    );

    Task<UserProfile> UpdateUserProfile(
        int userId,
        string firstName,
        string lastName,
        CancellationToken cts
    );

    Task<UserProfile> BlockUser(int userId, CancellationToken cts);
    Task<UserProfile> UnblockUser(int userId, CancellationToken cts);
    Task<UserProfile> DeleteUser(int userId, CancellationToken cts);
    Task<UserProfile> RestoreUser(int userId, CancellationToken cts);
    Task<bool> DoesUserExist(int userId, CancellationToken cts);
    Task<bool> DoesUserExist(string email, CancellationToken cts);
}
