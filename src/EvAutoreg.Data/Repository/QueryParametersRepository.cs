using System.Text;
using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class QueryParametersRepository : IQueryParametersRepository
{
    private readonly ISqlDataAccess _db;
    private readonly IFilterQueryBuilder _filterQueryBuilder;

    public QueryParametersRepository(ISqlDataAccess db, IFilterQueryBuilder filterQueryBuilder)
    {
        _db = db;
        _filterQueryBuilder = filterQueryBuilder;
    }

    public async Task<IEnumerable<QueryParametersModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sqlBuilder = new StringBuilder("SELECT * FROM registering_parameters");
        _filterQueryBuilder.ApplyPaginationFilter(sqlBuilder, filter, "issue_type_id");

        return await _db.LoadAllData<QueryParametersModel>(sqlBuilder.ToString(), cts);
    }

    public async Task<IEnumerable<QueryParametersModel>> Get(int issueTypeId, CancellationToken cts)
    {
        const string sql =
            @"SELECT * FROM registering_parameters WHERE issue_type_id = @IssueTypeId";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadData<QueryParametersModel>(sql, parameters, cts);
    }

    public async Task<QueryParametersModel> Add(
        QueryParametersModel queryParameters,
        CancellationToken cts
    )
    {
        const string sql =
            @"INSERT INTO registering_parameters (
                                    issue_type_id, work_time, status, assigned_group,
                                    request_type, execution_order)
                                    VALUES (@IssueTypeId, @WorkTime, @Status, @AssignedGroup,
                                            @RequestType, @ExecutionOrder)
                                    RETURNING *";

        var parameters = new DynamicParameters(queryParameters);
        return await _db.SaveData<QueryParametersModel>(sql, parameters, cts);
    }

    public async Task<QueryParametersModel> Update(
        QueryParametersModel queryParameters,
        CancellationToken cts
    )
    {
        const string sql =
            @"UPDATE registering_parameters SET 
                                  work_time = @WorkTime,
                                  status = @Status,
                                  assigned_group = @AssignedGroup,
                                  request_type = @RequestType,
                                  execution_order = @ExecutionOrder
                                  WHERE id = @Id AND issue_type_id = @IssueTypeId
                                  RETURNING *";

        var parameters = new DynamicParameters(queryParameters);
        return await _db.SaveData<QueryParametersModel>(sql, parameters, cts);
    }

    public async Task<QueryParametersModel> Delete(int id, int issueTypeId, CancellationToken cts)
    {
        const string sql =
            @"DELETE FROM registering_parameters WHERE id = @Id AND issue_type_id = @IssueTypeId RETURNING *";

        var parameters = new DynamicParameters(new { Id = id, IssueTypeId = issueTypeId });

        return await _db.SaveData<QueryParametersModel>(sql, parameters, cts);
    }

    public async Task<bool> DoQueryParametersExistFor(int issueTypeId, CancellationToken cts)
    {
        const string sql =
            @"SELECT EXISTS (SELECT true FROM registering_parameters WHERE issue_type_id = @IssueTypeId)";

        var parameters = new DynamicParameters(new { IssueTypeId = issueTypeId });

        return await _db.LoadSingle<bool>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from registering_parameters";
        return await _db.LoadScalar<int>(sql, cts);
    }
}