using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DataAccessLibrary.SqlDataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<TModel>> LoadAllData<TModel>(
        string sql,
        CancellationToken cts,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        return await connection.QueryAsync<TModel>(new CommandDefinition(sql, cancellationToken: cts)) ?? 
               Enumerable.Empty<TModel>();
    }
    
    public async Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));
        
        return await connection.QueryAsync<TModel>(new CommandDefinition(sql, parameters, cancellationToken: cts));
    }

    public async Task<TModel?> LoadFirst<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        return await connection.QueryFirstOrDefaultAsync<TModel?>(new CommandDefinition(sql, parameters, cancellationToken: cts));
    }
    
    public async Task SaveData<TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cts));
    }
}