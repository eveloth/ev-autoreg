using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IIssueService
{
    Task<IEnumerable<Issue>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<Issue> Get(int id, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}