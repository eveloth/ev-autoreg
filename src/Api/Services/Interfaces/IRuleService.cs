using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IRuleService
{
    Task<IEnumerable<Rule>> GetAll(PaginationQuery paginationQuery, CancellationToken cts);
    Task<Rule?> Get(int id, CancellationToken cts);
    Task<Rule> Add(Rule rule, CancellationToken cts);
    Task<Rule> Update(Rule rule, CancellationToken cts);
    Task<Rule> Delete(int id, CancellationToken cts);
}