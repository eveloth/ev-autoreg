using System.Text;
using EvAutoreg.Data.DataAccess;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Data.Repository;

public class GeneralPurposeRepository : IGeneralPurposeRepository
{
    private readonly ISqlDataAccess _db;

    public GeneralPurposeRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<bool> IsTableEmpty(string tableName, CancellationToken cts)
    {
        var sql = new StringBuilder("SELECT EXISTS (SELECT * FROM")
            .Append(' ')
            .Append(tableName)
            .Append(' ')
            .Append("LIMIT 1)")
            .ToString();

        var result = (await _db.LoadAllData<bool>(sql, cts)).First();

        return !result;
    }
}