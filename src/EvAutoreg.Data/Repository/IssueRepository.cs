﻿using System.Text;
using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class IssueRepository : IIssueRepository
{
    private readonly ISqlDataAccess _db;
    private readonly IFilterQueryBuilder _filterQueryBuilder;

    public IssueRepository(ISqlDataAccess db, IFilterQueryBuilder filterQueryBuilder)
    {
        _db = db;
        _filterQueryBuilder = filterQueryBuilder;
    }

    public async Task<IssueModel?> Get(int issueId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue WHERE id = @IssueId";

        var parameters = new DynamicParameters(new { IssueId = issueId });

        return await _db.LoadSingle<IssueModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    )
    {
        var sqlBuilder = new StringBuilder("SELECT * FROM issue");
        _filterQueryBuilder.ApplyPaginationFilter(sqlBuilder, filter, "id");

        return await _db.LoadAllData<IssueModel>(sqlBuilder.ToString(), cts);
    }

    public async Task<IssueModel> Upsert(IssueModel issue, CancellationToken cts)
    {
        const string sql =
            @"INSERT INTO issue
                             (id, time_created, author, company, status, priority, assigned_group, assignee, 
                              short_description, description, registrar_id, issue_type_id)
                             VALUES
                             (@Id, @TimeCreated, @Author, @Company, @Status, @Priority, @AssignedGroup, @Assignee,
                              @ShortDescription, @Description, @RegistrarId, @IssueTypeId)
                             ON CONFLICT (id)
                             DO UPDATE SET
                             author = excluded.author,
                             company = excluded.company,
                             status = excluded.status,
                             priority = excluded.priority,
                             assigned_group = excluded.assigned_group,
                             assignee = excluded.assignee
                             RETURNING *";

        var parameters = new DynamicParameters(issue);

        return await _db.SaveData<IssueModel>(sql, parameters, cts);
    }

    public async Task<IssueModel> Delete(int issueId, CancellationToken cts)
    {
        const string sql = @"DELETE FROM issue WHERE id = @IssueId RETURNING *";

        var parameters = new DynamicParameters(new { IssueId = issueId });

        return await _db.SaveData<IssueModel>(sql, parameters, cts);
    }

    public async Task<int> Count(CancellationToken cts)
    {
        const string sql = "SELECT COUNT(*) from issue";
        return await _db.LoadScalar<int>(sql, cts);
    }
}