using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserModel>> GetAll(
        CancellationToken cts,
        bool includeDeleted = false,
        PaginationFilter? filter = null
    );
    Task<UserModel?> GetById(int userId, CancellationToken cts, bool includeDeleted = false);
    Task<UserModel?> GetByEmail(string email, CancellationToken cts, bool includeDeleted = false);
    Task<UserModel> Create(UserModel user, CancellationToken cts);
    Task<int> UpdatePassword(UserModel user, CancellationToken cts);
    Task<UserModel> UpdateEmail(UserModel user, CancellationToken cts);
    Task<UserModel> UpdateUserProfile(UserModel user, CancellationToken cts);
    Task<UserModel> AddUserToRole(UserModel user, CancellationToken cts);
    Task<UserModel> RemoveUserFromRole(int userId, CancellationToken cts);
    Task<UserModel> Block(int userId, CancellationToken cts);
    Task<UserModel> Unblock(int userId, CancellationToken cts);
    Task<UserModel> Delete(int userId, CancellationToken cts);
    Task<UserModel> Restore(int userId, CancellationToken cts);
    Task<bool> DoesExist(int userId, CancellationToken cts);
    Task<bool> DoesExist(string email, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}