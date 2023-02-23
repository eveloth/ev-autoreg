using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IRuleService
{
    Task<IEnumerable<Rule>> GetAll(int userId, PaginationQuery paginationQuery, CancellationToken cts);
    Task<Rule> Get(int ruleId, int userId, CancellationToken cts);
    Task<Rule> Add(Rule rule, CancellationToken cts);
    Task<Rule> Update(Rule rule, CancellationToken cts);
    Task<Rule> Delete(int ruleId, int userId, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}