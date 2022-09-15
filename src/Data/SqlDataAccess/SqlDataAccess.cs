using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;

namespace Data.DataAccess.SqlDataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<TModel>> LoadAll<TModel>(
        string procedure,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        return await connection.QueryAsync<TModel>(procedure, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string procedure,
        TParameters parameters,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        return await connection.QueryAsync<TModel>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }
    public async Task SaveData<TParameters>(
        string procedure,
        TParameters parameters,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        await connection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
    }
}