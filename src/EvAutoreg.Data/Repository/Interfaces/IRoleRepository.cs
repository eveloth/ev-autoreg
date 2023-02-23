using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<RoleModel?> Get(int id, CancellationToken cts);
    Task<RoleModel> Add(RoleModel role, CancellationToken cts);
    Task<RoleModel> ChangeName(RoleModel role, CancellationToken cts);
    Task<RoleModel> ChangePriveleges(RoleModel role, CancellationToken cts);
    Task<RoleModel> Delete(int roleId, CancellationToken cts);
    Task<bool> DoesExist(int roleId, CancellationToken cts);
    Task<bool> DoesExist(string roleName, CancellationToken cts);
}