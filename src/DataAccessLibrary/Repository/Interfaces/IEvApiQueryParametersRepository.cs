using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IEvApiQueryParametersRepository
{
    Task<EvApiQueryParametersModel?> Get(int issueTypeId, CancellationToken cts);
    Task<IEnumerable<EvApiQueryParametersModel>> GetAll(
        PaginationFilter filter,
        CancellationToken cts
    );
    Task<EvApiQueryParametersModel> Upsert(
        EvApiQueryParametersModel queryParameters,
        CancellationToken cts
    );
    Task<EvApiQueryParametersModel> Delete(int issueTypeId, CancellationToken cts);
    Task<bool> DoQueryParametersExistFor(int issueTypeId, CancellationToken cts);
}