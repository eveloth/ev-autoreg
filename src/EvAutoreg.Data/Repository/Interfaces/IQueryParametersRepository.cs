using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IQueryParametersRepository
{
    Task<IEnumerable<QueryParametersModel>> GetAll(CancellationToken cts, PaginationFilter? filter = null);
    Task<IEnumerable<QueryParametersModel>> Get(int issueTypeId, CancellationToken cts);
    Task<QueryParametersModel> Add(QueryParametersModel queryParameters, CancellationToken cts);
    Task<QueryParametersModel> Update(QueryParametersModel queryParameters, CancellationToken cts);
    Task<QueryParametersModel> Delete(int id, int issueTypeId, CancellationToken cts);
    Task<bool> DoQueryParametersExistFor(int issueTypeId, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}