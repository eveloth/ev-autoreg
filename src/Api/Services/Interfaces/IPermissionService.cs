using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<Permission>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
}