using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class IssueFieldRepository : IIssueFieldRepository
{
    private readonly ISqlDataAccess _db;

    public IssueFieldRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IssueFieldModel?> GetIssueField(int issueFieldId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue_field WHERE id = @IssueFieldId";

        var parameters = new DynamicParameters(new { IssueFieldId = issueFieldId });

        return await _db.LoadFirst<IssueFieldModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueFieldModel>> GetAllIssueFields(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.PageSize;
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var sql = @$"SELECT * FROM issue_field ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<IssueFieldModel>(sql, cts);
    }

    public async Task AddIssueField(string issueFieldName, CancellationToken cts)
    {
        const string sql = "INSERT INTO issue_field (field_name) VALUES (@IssueFieldName) RETURNING id";

        var parameters = new DynamicParameters(new {IssueFieldName = issueFieldName});

        await _db.SaveData(sql, parameters, cts);
    }

    public async Task<bool> DoesIssueFieldExist(int issueFieldId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM issue_field WHERE id = @IssueFieldId)";

        var parameters = new DynamicParameters(new { IssueFieldId = issueFieldId });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}