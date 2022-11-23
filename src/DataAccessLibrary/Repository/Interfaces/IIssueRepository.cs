using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueRepository
{
    Task<IssueModel?> GetIssue(int issueId, CancellationToken cts);
    Task<IEnumerable<IssueModel>> GetAllIssues(PaginationFilter filter, CancellationToken cts);
    Task<IssueModel> UpsertIssue(IssueModel issue, CancellationToken cts);
    Task<IssueModel> DeleteIssue(int issueId, CancellationToken cts);
}