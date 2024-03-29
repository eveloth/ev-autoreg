using System.Text;
using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class IssueFieldRepository : IIssueFieldRepository
{
    private readonly ISqlDataAccess _db;
    private readonly IFilterQueryBuilder _filterQueryBuilder;

    public IssueFieldRepository(ISqlDataAccess db, IFilterQueryBuilder filterQueryBuilder)
    {
        _db = db;
        _filterQueryBuilder = filterQueryBuilder;
    }

    public async Task<IssueFieldModel?> Get(int issueFieldId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue_field WHERE id = @IssueFieldId";

        var parameters = new DynamicParameters(new { IssueFieldId = issueFieldId });

        return await _db.LoadSingle<IssueFieldModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueFieldModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sqlBuilder = new StringBuilder("SELECT * FROM issue_field");
        _filterQueryBuilder.ApplyPaginationFilter(sqlBuilder, filter, "id");

        return await _db.LoadAllData<IssueFieldModel>(sqlBuilder.ToString(), cts);
    }

    public async Task Add(IssueFieldModel issueField, CancellationToken cts)
    {
        const string sql = "INSERT INTO issue_field (field_name) VALUES (@FieldName) RETURNING id";

        var parameters = new DynamicParameters(issueField);

        await _db.SaveData(sql, parameters, cts);
    }

    public async Task Delete(int issueFieldId, CancellationToken cts)
    {
        const string sql = "DELETE FROM issue_field WHERE id = @IssueFieldId";

        var parameters = new DynamicParameters(new { IssueFieldId = issueFieldId });

        await _db.SaveData(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(int issueFieldId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM issue_field WHERE id = @IssueFieldId)";

        var parameters = new DynamicParameters(new { IssueFieldId = issueFieldId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from issue_field";
        return await _db.LoadScalar<int>(sql, cts);
    }
}