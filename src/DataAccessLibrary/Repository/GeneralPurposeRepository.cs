using System.Data.Common;
using System.Text;
using Dapper;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repositories;

public class GeneralPurposeRepository : IGeneralPurposeRepository
{
    private readonly ISqlDataAccess _db;
    private readonly DbTransaction _transaction;

    public GeneralPurposeRepository(ISqlDataAccess db, DbTransaction transaction)
    {
        _db = db;
        _transaction = transaction;
    }

    public async Task<bool> IsTableEmpty(string tableName, CancellationToken cts)
    {
        var sql = new StringBuilder("SELECT EXISTS (SELECT * FROM")
            .Append(' ')
            .Append(tableName)
            .Append(' ')
            .Append("LIMIT 1)").ToString();

        Console.WriteLine(sql);

        var result = (await _db.LoadAllData<bool>(sql, cts)).First();

        await _transaction.CommitAsync(cts);

        return result;
    }
}