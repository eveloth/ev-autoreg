using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class RuleSetRepository : IRuleSetRepository
{
    private readonly ISqlDataAccess _db;

    public RuleSetRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<FilledRuleSetModel>> GetAllForIssueType(
        int ownerUserId,
        int issueTypeId,
        CancellationToken cts
    )
    {
        const string selectRules =
            @"SELECT r.id as id, rule_set_id, 
                    rule, is_regex, is_negative, 
                    i.id as id, field_name 
                    FROM rule_set rs
                    JOIN rule r ON
                    rs.id = r.rule_set_id
                    JOIN issue_field i ON
                    issue_field_id = i.id
                    WHERE owner_user_id = @UserId
                    AND issue_type_id = @IssueTypeId";

        const string selectRuleSets =
            @"SELECT * FROM rule_set rs
                    JOIN issue_type t
                    ON rs.issue_type_id = t.id
                    WHERE owner_user_id = @UserId 
                    AND issue_type_id = @IssueTypeId";

        var parameters = new DynamicParameters(
            new { UserId = ownerUserId, IssueTypeId = issueTypeId }
        );

        var rules = await _db.LoadData<FilledRuleModel, IssueFieldModel>(
            selectRules,
            parameters,
            cts
        );
        var ruleSets = await _db.LoadData<FilledRuleSetModel, IssueTypeModel>(
            selectRuleSets,
            parameters,
            cts
        );

        var result = ruleSets.Select(x =>
        {
            var relatedRules = rules.Where(r => r.RuleSetId == x.Id).ToList();
            x.Rules = relatedRules;
            return x;
        });

        return result;
    }

    public async Task<FilledRuleSetModel?> Get(int ruleSetId, CancellationToken cts)
    {
        const string selectRules =
            @"SELECT r.id as id, rule_set_id, 
                    rule, is_regex, is_negative, 
                    i.id as id, field_name 
                    FROM rule_set rs
                    JOIN rule r ON
                    rs.id = r.rule_set_id
                    JOIN issue_field i ON
                    issue_field_id = i.id
                    WHERE rule_set_id = @RuleSetId";

        const string selectRuleSet =
            @"SELECT * FROM rule_set rs
                    JOIN issue_type t
                    ON rs.issue_type_id = t.id
                    WHERE rs.id = @RuleSetId";

        var parameters = new DynamicParameters(new { RuleSetId = ruleSetId });
        var rules = await _db.LoadData<FilledRuleModel, IssueFieldModel>(
            selectRules,
            parameters,
            cts
        );
        var ruleSet = await _db.LoadSingle<FilledRuleSetModel, IssueTypeModel>(
            selectRuleSet,
            parameters,
            cts
        );

        ruleSet!.Rules = rules.ToList();
        return ruleSet;
    }

    public async Task<IEnumerable<FilledRuleSetModel>> Add(
        RuleSetModel ruleSet,
        CancellationToken cts
    )
    {
        const string sql =
            "INSERT INTO rule_set (owner_user_id, issue_type_id)"
            + "VALUES (@OwnerUserId, @IssueTypeId)"
            + "RETURNING *";

        var parameters = new DynamicParameters(ruleSet);
        var createdRuleSet = await _db.SaveData<RuleSetModel>(sql, parameters, cts);

        var updatedRuleSets = await GetAllForIssueType(
            createdRuleSet.OwnerUserId,
            createdRuleSet.IssueTypeId,
            cts
        );
        return updatedRuleSets;
    }

    public async Task<FilledRuleModel?> GetEntry(int ruleSetId, int ruleId, CancellationToken cts)
    {
        const string sql =
            @"SELECT * from rule r
              JOIN issue_field i ON
              r.issue_field_id = i.id
              WHERE rule_set_id = @RuleSetId AND r.id = @RuleId";

        var parameters = new DynamicParameters(new { RuleSetId = ruleSetId, RuleId = ruleId });
        return await _db.LoadSingle<FilledRuleModel, IssueFieldModel>(sql, parameters, cts);
    }

    public async Task<FilledRuleSetModel> AddEntry(RuleModel rule, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO rule (rule_set_id, rule, issue_field_id, is_regex, is_negative)
              VALUES (@RuleSetId, @Rule, @IssueFieldId, @IsRegex, @IsNegative)
              RETURNING id";

        var parameters = new DynamicParameters(rule);
        await _db.SaveData<int>(sql, parameters, cts);
        var updatedRuleSet = await Get(rule.RuleSetId, cts);
        return updatedRuleSet!;
    }

    public async Task<FilledRuleSetModel> UpdateEntry(RuleModel rule, CancellationToken cts)
    {
        const string sql =
            @"UPDATE rule SET
            rule_set_id = @RuleSetId, rule = @Rule,
            issue_field_id = @IssueFieldId,
            is_regex = @IsRegex, is_negative = @IsNegative
            WHERE id = @Id RETURNING id";

        var parameters = new DynamicParameters(rule);
        await _db.SaveData<int>(sql, parameters, cts);
        var updatedRuleSet = await Get(rule.RuleSetId, cts);
        return updatedRuleSet!;
    }

    public async Task<FilledRuleSetModel> DeleteEntry(int ruleId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM rule WHERE id = @Id RETURNING *";

        var parameters = new DynamicParameters(new { Id = ruleId });
        var deletedRule = await _db.SaveData<FilledRuleModel>(sql, parameters, cts);
        var updatedRuleSet = await Get(deletedRule.RuleSetId, cts);
        return updatedRuleSet!;
    }

    public async Task<IEnumerable<FilledRuleSetModel>> Delete(
        int ownerUserId,
        int ruleSetId,
        CancellationToken cts
    )
    {
        const string sql =
            @"DELETE FROM rule_set WHERE 
                  id = @RuleSetId AND owner_user_id = @OwnerUserId 
                  RETURNING *";

        var parameters = new DynamicParameters(
            new { RuleSetId = ruleSetId, OwnerUserId = ownerUserId }
        );
        var deletedRuleSet = await _db.SaveData<RuleSetModel>(sql, parameters, cts);

        var updatedRuleSets = await GetAllForIssueType(
            deletedRuleSet.OwnerUserId,
            deletedRuleSet.IssueTypeId,
            cts
        );
        return updatedRuleSets;
    }

    public async Task<bool> DoesRuleSetExist(int ownerUserId, int ruleSetId, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM rule_set 
              WHERE owner_user_id = @OwnerUserId AND id = @RuleSetId)";

        var parameters = new DynamicParameters(
            new { OwnerUserId = ownerUserId, @RuleSetId = ruleSetId }
        );
        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }
}