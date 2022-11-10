using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repositories;

public interface IUserRepository
{
    Task<bool> DoesUserExist(int id, CancellationToken cts);
    Task<bool> DoesUserExist(string email, CancellationToken cts);

    Task<IEnumerable<UserModel>> GetAllUsers(
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserModel?> GetUserById(
        int id,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserModel?> GetUserByEmail(
        string email,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfileModel> CreateUser(UserModel user, CancellationToken cts);

    Task<IEnumerable<UserProfileModel>> GetAllUserProfiles(
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfileModel?> GetUserProfle(
        int id,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<int> UpdateUserPassword(int id, string passwordHash, CancellationToken cts);
    Task<UserProfileModel> UpdateUserEmail(int id, string newEmail, CancellationToken cts);

    Task<UserProfileModel> UpdateUserProfile(
        int id,
        string firstName,
        string lastName,
        CancellationToken cts
    );

    Task<int> BlockUser(int id, CancellationToken cts);
    Task<int> UnblockUser(int id, CancellationToken cts);
    Task<UserProfileModel> DeleteUser(int id, CancellationToken cts);

    Task<NewUserModel?> GetNewUserModel(
        int id,
        CancellationToken cts
    );
}
