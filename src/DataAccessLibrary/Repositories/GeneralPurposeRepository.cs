using System.Text;
using Dapper;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repositories;

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
            .Append("LIMIT 1)").ToString();

        Console.WriteLine(sql);

        return (await _db.LoadAllData<bool>(sql, cts)).First();
    }
}