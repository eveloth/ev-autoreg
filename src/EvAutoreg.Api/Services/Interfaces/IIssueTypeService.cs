using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IIssueTypeService
{
    Task<IEnumerable<IssueType>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<IssueType> Get(int id, CancellationToken cts);
    Task<IssueType> Add(IssueType type, CancellationToken cts);
    Task<IssueType> Rename(IssueType type, CancellationToken cts);
    Task<IssueType> Delete(int id, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}