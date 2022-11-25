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

    public async Task<IssueFieldModel?> GetIssueFiled(int issueFieldId, CancellationToken cts)
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
        var take = filter.Pagesize;
        var skip = (filter.PageNumber - 1) * filter.Pagesize;

        var sql = @$"SELECT * FROM issue_field ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<IssueFieldModel>(sql, cts);
    }
}
