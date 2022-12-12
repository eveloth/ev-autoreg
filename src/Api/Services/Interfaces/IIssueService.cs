using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IIssueService
{
    Task<IEnumerable<Issue>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<Issue?> Get(int id, CancellationToken cts);
}