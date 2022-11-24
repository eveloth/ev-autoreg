using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IEvApiQueryParametersRepository
{
    Task<EvApiQueryParametersModel?> GetQueryParameters(int issueTypeId, CancellationToken cts);
    Task<IEnumerable<EvApiQueryParametersModel>> GetAllQueryParameters(
        PaginationFilter filter,
        CancellationToken cts
    );
    Task<EvApiQueryParametersModel> UpsertQueryParameters(
        EvApiQueryParametersModel queryParameters,
        CancellationToken cts
    );
    Task<EvApiQueryParametersModel> DeleteQueryParameters(int issueTypeId, CancellationToken cts);
    Task<bool> DoQueryParametersExistFor(int issueTypeId, CancellationToken cts);
}
