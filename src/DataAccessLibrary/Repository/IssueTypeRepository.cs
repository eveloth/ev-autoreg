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

    public async Task<IssueTypeModel?> Get(int issueTypeId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue_type WHERE issue_type.id = @IssueTypeId";
        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadFirst<IssueTypeModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueTypeModel>> GetAll(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.PageSize;
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var sql = @$"SELECT * FROM issue_type ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<IssueTypeModel>(sql, cts);
    }

    public async Task<IssueTypeModel> Add(string issueTypeName, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO issue_type (issue_type_name) VALUES (@IssueTypeName) RETURNING *";

        var parameters = new DynamicParameters(new { IssueTypeName = issueTypeName });

        return await _db.SaveData<IssueTypeModel>(sql, parameters, cts);
    }

    public async Task<IssueTypeModel> ChangeName(
        int issueTypeId,
        string issueTypeName,
        CancellationToken cts
    )
    {
        const string sql =
            @"UPDATE issue_type SET issue_type_name = @IssueTypeName WHERE id = @IssueTypeId RETURNING *";

        var parameters = new DynamicParameters(
            new { IssueTypeId = issueTypeId, IssueTypeName = issueTypeName }
        );

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

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }

    public async Task<bool> DoesExist(string issueTypeName, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM issue_type WHERE issue_type_name = @IssueTypeName)";

        var parameters = new DynamicParameters(new { IssueTypeName = issueTypeName });

        return await _db.LoadFirst<bool>(sql, parameters, cts);
    }
}