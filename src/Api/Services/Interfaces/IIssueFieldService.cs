using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IIssueFieldService
{
    Task<IEnumerable<IssueField>> GetAll(PaginationQuery query, CancellationToken cts);
}