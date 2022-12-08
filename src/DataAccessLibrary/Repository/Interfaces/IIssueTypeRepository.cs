using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IIssueTypeRepository
{
    Task<IssueTypeModel?> Get(int issueTypeId, CancellationToken cts);
    Task<IEnumerable<IssueTypeModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<IssueTypeModel> Add(string issueTypeName, CancellationToken cts);
    Task<IssueTypeModel> ChangeName(int issueTypeId, string issueTypeName, CancellationToken cts);
    Task<IssueTypeModel> Delete(int issueTypeId, CancellationToken cts);
    Task<bool> DoesExist(int issueTypeId, CancellationToken cts);
    Task<bool> DoesExist(string issueTypeName, CancellationToken cts);
}