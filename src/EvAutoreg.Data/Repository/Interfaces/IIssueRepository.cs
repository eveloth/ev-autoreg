using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IIssueRepository
{
    Task<IEnumerable<IssueModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<IssueModel?> Get(int issueId, CancellationToken cts);
    Task<IssueModel> Upsert(IssueModel issue, CancellationToken cts);
    Task<IssueModel> Delete(int issueId, CancellationToken cts);
}