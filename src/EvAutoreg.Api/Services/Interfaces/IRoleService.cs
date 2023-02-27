using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RolePermission>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<RolePermission> Get(int id, CancellationToken cts);
    Task<RolePermission> Add(Role role, CancellationToken cts);
    Task<RolePermission> Rename(Role role, CancellationToken cts);
    Task<RolePermission> Delete(int id, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}