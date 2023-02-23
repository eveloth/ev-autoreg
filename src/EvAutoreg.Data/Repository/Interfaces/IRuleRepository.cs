using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IRuleRepository
{
    Task<IEnumerable<RuleModel>> GetAll(int userId, CancellationToken cts, PaginationFilter? filter = null);
    Task<RuleModel?> Get(int ruleId, int userId, CancellationToken cts);
    Task<RuleModel> Add(RuleModel rule, CancellationToken cts);
    Task<RuleModel> Update(RuleModel rule, CancellationToken cts);
    Task<RuleModel> Delete(int ruleId, int userId, CancellationToken cts);
    Task<bool> DoesExist(int ruleId, int userId, CancellationToken cts);
}