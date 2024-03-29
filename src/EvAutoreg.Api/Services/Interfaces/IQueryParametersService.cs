using EvAutoreg.Api.Contracts.Queries;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IQueryParametersService
{
    Task<IEnumerable<QueryParameters>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<IEnumerable<QueryParameters>> Get(int issueTypeId, CancellationToken cts);
    Task<QueryParameters> Add(QueryParameters parameters, CancellationToken cts);
    Task<QueryParameters> Update(QueryParameters parameters, CancellationToken cts);
    Task<QueryParameters> Delete(int id, int issueTypeId, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}