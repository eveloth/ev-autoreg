using EvAutoreg.Api.Contracts.Queries;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<User> Get(int id, CancellationToken cts);
    Task<User> Get(string email, CancellationToken cts);
    Task<User> UpdateProfile(User user, CancellationToken cts);
    Task<User> ChangeEmail(User user, CancellationToken cts);
    Task<int> ChangePassword(int id, string password, CancellationToken cts);
    Task<User> AddUserToRole(User user, CancellationToken cts);
    Task<User> AddUserToPrivelegedRole(User user, CancellationToken cts);
    Task<User> RemoveUserFromRole(int id, CancellationToken cts);
    Task<User> RemoveUserFromPrivelegedRole(int id, CancellationToken cts);
    Task<User> Block(int id, CancellationToken cts);
    Task<User> Unblock(int id, CancellationToken cts);
    Task<User> Delete(int id, CancellationToken cts);
    Task<User> Restore(int id, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}