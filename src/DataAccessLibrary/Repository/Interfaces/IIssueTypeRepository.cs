using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueTypeRepository
{
    Task<IssueTypeModel?> GetIssueType(int issueTypeId, CancellationToken cts);
    Task<IEnumerable<IssueTypeModel>> GetAllIssueTypes(
        PaginationFilter filter,
        CancellationToken cts
    );
    Task<IssueTypeModel> AddIssueType(string issueTypeName, CancellationToken cts);
    Task<IssueTypeModel> ChangeIssueTypeName(int issueTypeId, string issueTypeName, CancellationToken cts);
    Task<IssueTypeModel> DeleteIssueType(int issueTypeId, CancellationToken cts);
    Task<bool> DoesIssueTypeExist(int issueTypeId, CancellationToken cts);
    Task<bool> DoesIssueTypeExist(string issueTypeName, CancellationToken cts);
}