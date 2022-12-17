using Api.Contracts;
using Api.Domain;

namespace Api.Services.Interfaces;

public interface IRuleService
{
    Task<IEnumerable<Rule>> GetAll(int userId, PaginationQuery paginationQuery, CancellationToken cts);
    Task<Rule> Get(int ruleId, int userId, CancellationToken cts);
    Task<Rule> Add(Rule rule, CancellationToken cts);
    Task<Rule> Update(Rule rule, CancellationToken cts);
    Task<Rule> Delete(int ruleId, int userId, CancellationToken cts);
}