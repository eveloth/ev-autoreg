using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueFieldRepository
{
    Task<IssueFieldModel?> GetIssueField(int issueFieldId, CancellationToken cts);
    Task<IEnumerable<IssueFieldModel>> GetAllIssueFields(PaginationFilter filter, CancellationToken cts);
    Task AddIssueField(string issueFieldName, CancellationToken cts);
    Task<bool> DoesIssueFieldExist(int issueFieldId, CancellationToken cts);
}