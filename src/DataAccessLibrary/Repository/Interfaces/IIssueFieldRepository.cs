using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueFieldRepository
{
    Task<IssueFieldModel?> Get(int issueFieldId, CancellationToken cts);
    Task<IEnumerable<IssueFieldModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task Add(IssueFieldModel issueField, CancellationToken cts);
    Task<bool> DoesExist(int issueFieldId, CancellationToken cts);
}