using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class MailAnalysisRulesRepository : IMailAnalysisRulesRepository
{
    private readonly ISqlDataAccess _db;

    public MailAnalysisRulesRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<MailAnalysisRuleModel?> GetMailAnalysisRules(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM mail_analysis_rules WHERE id = 1";

        var result = (await _db.LoadAllData<MailAnalysisRuleModel?>(sql, cts)).FirstOrDefault();

        return result;
    }

    public async Task<MailAnalysisRuleModel> UpsertMailAnalysisRules(MailAnalysisRuleModel rule, CancellationToken cts)
    {
        const string sql = @"INSERT INTO mail_analysis_rules 
                             (id, new_issue_regex, issue_no_regex)
                             VALUES(1, @NewIssueRegex, @IssueNoRegex)
                             ON CONFLICT (id)
                             DO UPDATE SET
                             new_issue_regex = excluded.new_issue_regex, 
                             issue_no_regex = excluded.issue_no_regex
                             RETURNING * ";

        var parameters = new DynamicParameters(rule);

        return await _db.SaveData<MailAnalysisRuleModel>(sql, parameters, cts);
    }

    public async Task<MailAnalysisRuleModel> DeleteMailAnalysisRules(CancellationToken cts)
    {
        const string sql = "DELETE FROM mail_analysis_rules RETURNING *";

        return await _db.SaveData<MailAnalysisRuleModel>(sql, cts);
    }

    public async Task<bool> DoMailAnalysisRulesExist(CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM mail_analysis_rules WHERE id = 1)";

        return await _db.LoadFirst<bool>(sql, cts);
    }
}