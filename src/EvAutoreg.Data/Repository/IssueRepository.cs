﻿using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class IssueRepository : IIssueRepository
{
    private readonly ISqlDataAccess _db;

    public IssueRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<IssueModel?> Get(int issueId, CancellationToken cts)
    {
        const string sql = @"SELECT * FROM issue WHERE id = @IssueId";

        var parameters = new DynamicParameters(new { IssueId = issueId });

        return await _db.LoadSingle<IssueModel?>(sql, parameters, cts);
    }

    public async Task<IEnumerable<IssueModel>> GetAll(
        PaginationFilter filter,
        CancellationToken cts
    )
    {
        var take = filter.PageSize;
        var skip = (filter.PageNumber - 1) * filter.PageSize;

        var sql = @$"SELECT * FROM issue ORDER BY id LIMIT {take} OFFSET {skip}";

        return await _db.LoadAllData<IssueModel>(sql, cts);
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
}