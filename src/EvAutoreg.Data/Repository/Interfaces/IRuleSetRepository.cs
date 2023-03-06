using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IRuleSetRepository
{
    Task<IEnumerable<FilledRuleSetModel>> GetAllForIssueType(
        int ownerUserId,
        int issueTypeId,
        CancellationToken cts
    );

    Task<FilledRuleSetModel?> Get(int ruleSetId, CancellationToken cts);
    Task<IEnumerable<FilledRuleSetModel>> Add(RuleSetModel ruleSet, CancellationToken cts);
    Task<FilledRuleModel?> GetEntry(int ruleSetId, int ruleId, CancellationToken cts);
    Task<FilledRuleSetModel> AddEntry(RuleModel rule, CancellationToken cts);
    Task<FilledRuleSetModel> UpdateEntry(RuleModel rule, CancellationToken cts);
    Task<FilledRuleSetModel> DeleteEntry(int ruleSetId, CancellationToken cts);
    Task<IEnumerable<FilledRuleSetModel>> Delete(int ownerUserId, int ruleSetId, CancellationToken cts);
    Task<bool> DoesRuleSetExist(int ownerUserId, int ruleSetId, CancellationToken cts);
}