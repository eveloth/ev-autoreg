using System.Data;
using Dapper;
using DataAccessLibrary.Models;
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
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        return await connection.QueryAsync<TModel>(sql) ?? Enumerable.Empty<TModel>();
    }
    
    public async Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));
        
        return await connection.QueryAsync<TModel>(sql, parameters);
    }

    public async Task<TModel?> LoadFirst<TModel, TParameters>(
        string sql,
        TParameters parameters,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        var result = await connection.QueryAsync <TModel>(sql, parameters);

        return result.FirstOrDefault();
    }
    
    public async Task SaveData<TParameters>(
        string sql,
        TParameters parameters,
        string connectionId = "Default")
    {
        using IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionId));

        await connection.ExecuteAsync(sql, parameters);
    }
}