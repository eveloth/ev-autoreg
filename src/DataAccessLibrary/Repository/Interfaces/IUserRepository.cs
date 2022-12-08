using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IUserRepository
{
    Task<UserModel?> GetById(int userId, CancellationToken cts, bool includeDeleted = false);

    Task<UserModel?> GetByEmail(string email, CancellationToken cts, bool includeDeleted = false);

    Task<IEnumerable<UserProfileModel>> GetAllUserProfiles(
        PaginationFilter filter,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfileModel?> GetUserProfle(
        int userId,
        CancellationToken cts,
        bool includeDeleted = false
    );

    Task<UserProfileModel> Create(UserModel user, CancellationToken cts);

    Task<int> UpdatePassword(int userId, string passwordHash, CancellationToken cts);

    Task<UserProfileModel> UpdateEmail(int userId, string newEmail, CancellationToken cts);

    Task<UserProfileModel> UpdateUserProfile(
        int userId,
        string firstName,
        string lastName,
        CancellationToken cts
    );

    Task<UserProfileModel> Block(int userId, CancellationToken cts);
    Task<UserProfileModel> Unblock(int userId, CancellationToken cts);
    Task<UserProfileModel> Delete(int userId, CancellationToken cts);
    Task<UserProfileModel> Restore(int userId, CancellationToken cts);
    Task<bool> DoesExist(int userId, CancellationToken cts);
    Task<bool> DoesExist(string email, CancellationToken cts);
}