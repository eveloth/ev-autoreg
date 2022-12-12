using Api.Contracts;
using Api.Domain;
using NuGet.Packaging.Signing;

namespace Api.Services.Interfaces;

public interface IQueryParametersService
{
    Task<IEnumerable<QueryParameters>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<QueryParameters?> Get(QueryParameters parameters, CancellationToken cts);
    Task<QueryParameters> Upsert(QueryParameters parameters, CancellationToken cts);
}