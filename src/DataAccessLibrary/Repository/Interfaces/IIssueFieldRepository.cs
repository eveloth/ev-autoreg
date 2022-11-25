using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueFieldRepository
{
    Task<IssueFieldModel?> GetIssueFiled(int issueFieldId, CancellationToken cts);
    Task<IEnumerable<IssueFieldModel>> GetAllIssueFields(PaginationFilter filter, CancellationToken cts);
}