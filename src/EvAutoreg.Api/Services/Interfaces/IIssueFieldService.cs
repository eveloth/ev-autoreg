using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IIssueFieldService
{
    Task<IEnumerable<IssueField>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}