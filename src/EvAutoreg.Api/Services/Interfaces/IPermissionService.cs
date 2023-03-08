using EvAutoreg.Api.Contracts.Queries;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<Permission>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}