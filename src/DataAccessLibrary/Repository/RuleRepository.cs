using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class RuleRepository : IRuleRepository
{
    private readonly ISqlDataAccess _db;

    public RuleRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<RuleModel?> GetRule(int ruleId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM rule WHERE id = @RuleId";

        var parameters = new DynamicParameters(new { RuleId = ruleId });

        return await _db.LoadFirst<RuleModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<RuleModel>> GetAllRules(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.Pagesize;
        var skip = (filter.PageNumber - 1) * filter.Pagesize;

        var sql = @$"SELECT * FROM rule ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<RuleModel>(sql, cts);
    }

    public async Task<RuleModel> AddRule(RuleModel rule, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO rule (rule, owner_user_id, issue_type_id, 
                                               issue_field_id, is_regex, is_negative)
                                               VALUES 
                                               (@Rule, @OwnerUserId, @IssueTypeId,
                                               @IssueFieldId, @IsRegex, @IsNegative)
                                               RETURNING *";

        var parameters = new DynamicParameters(rule);

        return await _db.SaveData<RuleModel>(sql, parameters, cts);
    }

    public async Task<RuleModel> UpdateRule(RuleModel rule, CancellationToken cts)
    {
        const string sql =
            @"UPDATE rule SET
                             rule = @Rule,
                             owner_user_id = @OwnerUserId,
                             issue_type_id = @IssueTypeId,
                             issue_field_id = @IssueFieldId,
                             is_regex = @IsRegex,
                             is_negative = @IsNegative
                             WHERE id = @Id
                             RETURNING *";

        var parameters = new DynamicParameters(rule);

        return await _db.SaveData<RuleModel>(sql, parameters, cts);
    }

    public async Task<RuleModel> DeleteRule(int ruleId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM rule WHERE id = @RuleId RETURNING *";

        var parameters = new DynamicParameters(new { RuleId = ruleId });

        return await _db.SaveData<RuleModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesRuleExist(int ruleId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM rule WHERE id = @RuleId)";

        var parameters = new DynamicParameters(new { RuleId = ruleId });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}
