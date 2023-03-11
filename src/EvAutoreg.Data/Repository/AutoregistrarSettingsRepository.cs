using Dapper;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class AutoregistrarSettingsRepository : IAutoregistrarSettingsRepository
{
    private readonly ISqlDataAccess _db;

    public AutoregistrarSettingsRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<AutoregstrarSettingsModel?> Get(CancellationToken cts)
    {
        const string sql = @"SELECT * FROM autoregistrar_settings";

        var result = await _db.LoadSingle<AutoregstrarSettingsModel?>(sql, cts);
        return result;
    }

    public async Task<AutoregstrarSettingsModel> Upsert(
        AutoregstrarSettingsModel settings,
        CancellationToken cts
    )
    {
        const string insert =
            @"INSERT INTO autoregistrar_settings 
                             (exchange_server_uri, extra_view_uri,
                             new_issue_regex, issue_no_regex)
                             VALUES(@ExchangeServerUri, @ExtraViewUri,
                             @NewIssueRegex, @IssueNoRegex)
                             RETURNING * ";

        const string update =
            @"UPDATE autoregistrar_settings SET
                     exchange_server_uri = @ExchangeServerUri,
                     extra_view_uri = @ExtraViewUri,
                     new_issue_regex = @NewIssueRegex, 
                     issue_no_regex = @IssueNoRegex
                     RETURNING *";

        var hasData = await DoExist(cts);
        var sql = hasData switch
        {
            true => update,
            false => insert
        };

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
        const string sql = @"SELECT EXISTS (SELECT true FROM autoregistrar_settings)";

        return await _db.LoadSingle<bool>(sql, cts);
    }
}