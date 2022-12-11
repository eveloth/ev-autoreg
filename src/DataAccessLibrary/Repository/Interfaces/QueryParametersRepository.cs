using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IQueryParametersRepository
{
    Task<IEnumerable<QueryParametersModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<QueryParametersModel?> Get(int issueTypeId, CancellationToken cts);
    Task<QueryParametersModel> Upsert(QueryParametersModel queryParameters, CancellationToken cts);
    Task<QueryParametersModel> Delete(int issueTypeId, CancellationToken cts);
    Task<bool> DoQueryParametersExistFor(int issueTypeId, CancellationToken cts);
}