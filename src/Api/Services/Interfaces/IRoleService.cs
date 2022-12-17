using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<Role> Get(int id, CancellationToken cts);
    Task<Role> Add(Role role, CancellationToken cts);
    Task<Role> Rename(Role role, CancellationToken cts);
    Task<Role> Delete(int id, CancellationToken cts);
}