using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRuleRepository
{
    Task<RuleModel?> Get(int ruleId, int userId, CancellationToken cts);
    Task<IEnumerable<RuleModel>> GetAll(int userId, PaginationFilter filter, CancellationToken cts);
    Task<RuleModel> Add(RuleModel rule, CancellationToken cts);
    Task<RuleModel> Update(RuleModel rule, CancellationToken cts);
    Task<RuleModel> Delete(int ruleId, int userId, CancellationToken cts);
    Task<bool> DoesExist(int ruleId, int userId, CancellationToken cts);
}