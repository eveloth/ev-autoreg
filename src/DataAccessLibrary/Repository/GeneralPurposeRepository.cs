using System.Text;
using DataAccessLibrary.Repository.Interfaces;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repository;

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

        Console.WriteLine(sql);

        var result = (await _db.LoadAllData<bool>(sql, cts)).First();

        return result;
    }
}
