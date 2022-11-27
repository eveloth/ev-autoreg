using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRuleRepository
{
    Task<RuleModel?> GetRule(int ruleId, int userId, CancellationToken cts);
    Task<IEnumerable<RuleModel>> GetAllRules(int userId, PaginationFilter filter, CancellationToken cts);
    Task<RuleModel> AddRule(RuleModel rule, CancellationToken cts);
    Task<RuleModel> UpdateRule(RuleModel rule, CancellationToken cts);
    Task<RuleModel> DeleteRule(int ruleId, int userId, CancellationToken cts);
    Task<bool> DoesRuleExist(int ruleId, int userId, CancellationToken cts);
}