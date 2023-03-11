using System.Text;
using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class IssueTypeRepository : IIssueTypeRepository
{
    private readonly ISqlDataAccess _db;
    private readonly IFilterQueryBuilder _filterQueryBuilder;

    public IssueTypeRepository(ISqlDataAccess db, IFilterQueryBuilder filterQueryBuilder)
    {
        _db = db;
        _filterQueryBuilder = filterQueryBuilder;
    }

    public async Task<IssueTypeModel?> Get(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue_type WHERE issue_type.id = @IssueTypeId";
        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadSingle<IssueTypeModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueTypeModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sqlBuilder = new StringBuilder("SELECT * FROM issue_type");
        _filterQueryBuilder.ApplyPaginationFilter(sqlBuilder, filter, "id");

        return await _db.LoadAllData<IssueTypeModel>(sqlBuilder.ToString(), cts);
    }

    public async Task<IssueTypeModel> Add(IssueTypeModel issueType, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO issue_type (issue_type_name) VALUES (@IssueTypeName) RETURNING *";

        var parameters = new DynamicParameters(issueType);

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<IssueTypeModel> ChangeName(IssueTypeModel issueType, CancellationToken cts)
    {
        const string sql =
            @"UPDATE issue_type SET issue_type_name = @IssueTypeName WHERE id = @Id RETURNING *";

        var parameters = new DynamicParameters(issueType);

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<IssueTypeModel> Delete(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM issue_type WHERE id = @IssueTypeId RETURNING *";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM issue_type WHERE id = @IssueTypeId)";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string issueTypeName, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM issue_type WHERE issue_type_name = @IssueTypeName)";

        var parameters = new DynamicParameters(new { IssueTypeName = issueTypeName });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from issue_type";
        return await _db.LoadScalar<int>(sql, cts);
    }
}