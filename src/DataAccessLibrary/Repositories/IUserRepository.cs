using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;

namespace DataAccessLibrary.Repositories;

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

    Task<IEnumerable<UserProfileModel>> GetAllUserProfiles(
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfileModel?> GetUserProfle(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfileModel> CreateUser(UserModel user, CancellationToken cts);

    Task<int> UpdateUserPassword(
        int userId,
        string passwordHash,
        CancellationToken cts
    );

    Task<UserProfileModel> UpdateUserEmail(
        int userId,
        string newEmail,
        CancellationToken cts
    );

    Task<UserProfileModel> UpdateUserProfile(
        int userId,
        string firstName,
        string lastName,
        CancellationToken cts
    );

    Task<UserProfileModel> BlockUser(int userId, CancellationToken cts);
    Task<UserProfileModel> UnblockUser(int userId, CancellationToken cts);
    Task<UserProfileModel> DeleteUser(int userId, CancellationToken cts);
    Task<UserProfileModel> RestoreUser(int userId, CancellationToken cts);
    Task<bool> DoesUserExist(int userId, CancellationToken cts);
    Task<bool> DoesUserExist(string email, CancellationToken cts);
}
