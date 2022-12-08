using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueFieldRepository
{
    Task<IssueFieldModel?> Get(int issueFieldId, CancellationToken cts);
    Task<IEnumerable<IssueFieldModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task Add(string issueFieldName, CancellationToken cts);
    Task<bool> DoesExist(int issueFieldId, CancellationToken cts);
}