using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IMailAnalysisRulesRepository
{
    Task<MailAnalysisRuleModel?> GetMailAnalysisRules(CancellationToken cts);
    Task<MailAnalysisRuleModel> UpsertMailAnalysisRules(MailAnalysisRuleModel rule, CancellationToken cts);
    Task<MailAnalysisRuleModel> DeleteMailAnalysisRules(CancellationToken cts);
    Task<bool> DoMailAnalysisRulesExist(CancellationToken cts);
}