using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class IssueTypeRepository : IIssueTypeRepository
{
    private readonly ISqlDataAccess _db;

    public IssueTypeRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IssueTypeModel?> GetIssueType(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue_type WHERE issue_type.id = @IssueTypeId";
        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadFirst<IssueTypeModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueTypeModel>> GetAllIssueTypes(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.Pagesize;
        var skip = (filter.PageNumber - 1) * filter.Pagesize;

        var sql = @$"SELECT * FROM issue_type ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<IssueTypeModel>(sql, cts);
    }

    public async Task<IssueTypeModel> AddIssueType(string issueTypeName, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO issue_type (issue_type_name) VALUES (@IssueTypeName) RETURNING *";

        var parameters = new DynamicParameters(new { IssueTypeName = issueTypeName });

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<IssueTypeModel> ChangeIssueTypeName(
        IssueTypeModel issueType,
        CancellationToken cts
    )
    {
        const string sql =
            @"UPDATE issue_type SET issue_type_name = @NewIssueTypeName WHERE id = @Id RETURNING *";

        var parameters = new DynamicParameters(issueType);

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<IssueTypeModel> DeleteIssueType(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM issue_type WHERE id = @IssueTypeId";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<bool> DoesIssueTypeExist(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM issue_type WHERE id = @IssueTypeId)";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesIssueTypeExist(string issueTypeName, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM issue_type WHERE issue_type_name = @IssueTypeName)";

        var parameters = new DynamicParameters(new { IssueTypeName = issueTypeName });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}
