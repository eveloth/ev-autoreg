using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class RuleRepository : IRuleRepository
{
    private readonly ISqlDataAccess _db;

    public RuleRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<RuleModel?> Get(int ruleId, int userId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM rule WHERE id = @RuleId AND owner_user_id = @UserId";

        var parameters = new DynamicParameters(new { RuleId = ruleId, UserId = userId });

        return await _db.LoadSingle<RuleModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<RuleModel>> GetAll(
        int userId,
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sql =
            @"SELECT * FROM rule WHERE owner_user_id = @UserId";

        if (filter is not null)
        {
            var take = filter.PageSize;
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var paginator = $" ORDER BY id LIMIT {take} offset {skip}";
            sql += paginator;
        }

        var parameters = new DynamicParameters(new { UserId = userId });

        return await _db.LoadData<RuleModel>(sql, parameters, cts);
    }

    public async Task<RuleModel> Add(RuleModel rule, CancellationToken cts)
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

    public async Task<RuleModel> Update(RuleModel rule, CancellationToken cts)
    {
        const string sql =
            @"UPDATE rule SET
                             rule = @Rule,
                             issue_type_id = @IssueTypeId,
                             issue_field_id = @IssueFieldId,
                             is_regex = @IsRegex,
                             is_negative = @IsNegative
                             WHERE id = @Id
                             AND owner_user_id = @OwnerUserId
                             RETURNING *";

        var parameters = new DynamicParameters(rule);

        return await _db.SaveData<RuleModel>(sql, parameters, cts);
    }

    public async Task<RuleModel> Delete(int ruleId, int userId, CancellationToken cts)
    {
        const string sql =
            @"DELETE FROM rule WHERE id = @RuleId AND owner_user_id = @UserId RETURNING *";

        var parameters = new DynamicParameters(new { RuleId = ruleId, UserId = userId });

        return await _db.SaveData<RuleModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(int ruleId, int userId, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM rule WHERE id = @RuleId AND owner_user_id = @UserId)";

        var parameters = new DynamicParameters(new { RuleId = ruleId, UserId = userId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }
}