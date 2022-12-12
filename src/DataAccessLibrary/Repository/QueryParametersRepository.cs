using Dapper;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class QueryParametersRepository : IQueryParametersRepository
{
    private readonly ISqlDataAccess _db;

    public QueryParametersRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<QueryParametersModel?> Get(int issueTypeId, CancellationToken cts)
    {
        const string sql =
            @"SELECT * FROM registering_parameters WHERE issue_type_id = @IssueTypeId";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadSingle<QueryParametersModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<QueryParametersModel>> GetAll(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.PageSize;
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var sql =
            @$"SELECT * FROM registering_parameters ORDER BY issue_type_id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<QueryParametersModel>(sql, cts);
    }

    public async Task<QueryParametersModel> Upsert(
        QueryParametersModel queryParameters,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO registering_parameters (
                                         issue_type_id, work_time, reg_status, 
                                         inwork_status, assigned_group, request_type)
                                         VALUES (
                                         @IssueTypeId, @WorkTime, @RegStatus, @InworkStatus,
                                         @AssignedGroup, @RequestType)
                                         ON CONFLICT (issue_type_id)
                                         DO UPDATE SET
                                         work_time = excluded.work_time,
                                         reg_status = excluded.reg_status,
                                         inwork_status = excluded.inwork_status,
                                         assigned_group = excluded.assigned_group,
                                         request_type = excluded.request_type
                                         RETURNING * ";

        var paratmeters = new DynamicParameters(queryParameters);

        return await _db.SaveData<QueryParametersModel>(sql, paratmeters, cts);
    }

    public async Task<QueryParametersModel> Delete(int issueTypeId, CancellationToken cts)
    {
        const string sql =
            @"DELETE FROM registering_parameters WHERE issue_type_id = @IssueTypeId RETURNING *";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.SaveData<QueryParametersModel>(sql, parameters, cts);
    }

    public async Task<bool> DoQueryParametersExistFor(int issueTypeId, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM registering_parameters WHERE issue_type_id = @IssueTypeId)";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }
}