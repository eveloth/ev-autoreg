using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

public class AutoregistrarSettingsRepository : IAutoregistrarSettingsRepository
{
    private readonly ISqlDataAccess _db;

    public AutoregistrarSettingsRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<AutoregstrarSettingsModel?> Get(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM autoregistrar_settings WHERE id = 1";

        var result = (await _db.LoadAllData<AutoregstrarSettingsModel?>(sql, cts)).FirstOrDefault();

        return result;
    }

    public async Task<AutoregstrarSettingsModel> Upsert(AutoregstrarSettingsModel settings, CancellationToken cts)
    {
        const string sql = @"INSERT INTO autoregistrar_settings 
                             (id, exchange_server_uri, extra_view_uri,
                             new_issue_regex, issue_no_regex)
                             VALUES(1, @ExchangeServerUri, @ExtraViewUri,
                             @NewIssueRegex, @IssueNoRegex)
                             ON CONFLICT (id)
                             DO UPDATE SET
                             exchange_server_uri = excluded.exchange_server_uri,
                             extra_view_uri = excluded.extra_view_uri,
                             new_issue_regex = excluded.new_issue_regex, 
                             issue_no_regex = excluded.issue_no_regex
                             RETURNING * ";

        var parameters = new DynamicParameters(settings);

        return await _db.SaveData<AutoregstrarSettingsModel>(sql, parameters, cts);
    }

    public async Task<AutoregstrarSettingsModel> Delete(CancellationToken cts)
    {
        const string sql = "DELETE FROM autoregistrar_settings RETURNING *";

        return await _db.SaveData<AutoregstrarSettingsModel>(sql, cts);
    }

    public async Task<bool> DoExist(CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM autoregistrar_settings WHERE id = 1)";

        return await _db.LoadFirst<bool>(sql, cts);
    }
}