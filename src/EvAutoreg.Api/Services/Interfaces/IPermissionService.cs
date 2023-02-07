using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<Permission>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
}