using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IQueryParametersService
{
    Task<IEnumerable<QueryParameters>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<QueryParameters> Get(int issueTypeId, CancellationToken cts);
    Task<QueryParameters> Upsert(QueryParameters parameters, CancellationToken cts);
}