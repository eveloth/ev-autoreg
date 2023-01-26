using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IQueryParametersService
{
    Task<IEnumerable<QueryParameters>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<QueryParameters> Get(int issueTypeId, CancellationToken cts);
    Task<QueryParameters> Upsert(QueryParameters parameters, CancellationToken cts);
}