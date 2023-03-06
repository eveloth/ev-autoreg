using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IRuleSetService
{
    Task<IEnumerable<RuleSet>> GetAllForIssueType(int ownerUserId, int issueTypeId, CancellationToken cts);
    Task<IEnumerable<RuleSet>> Add(RuleSet ruleSet, CancellationToken cts);
    Task<IEnumerable<RuleSet>> Delete(int ownerUserId, int ruleSetId, CancellationToken cts);
    Task<RuleSet> AddEntry(int ownerUserId, Rule rule, CancellationToken cts);
    Task<RuleSet> UpdateEntry(int ownerUserId, Rule rule, CancellationToken cts);
    Task<RuleSet> DeleteEntry(int ownerUserId, int ruleSetId, int ruleId, CancellationToken cts);
}