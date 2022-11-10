using System.Data;
using Dapper;
using Extensions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DataAccessLibrary.SqlDataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _config;

    public bool HasAffix { get; set; } = false;
    public string Affix { get; set; } = string.Empty;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<TModel>> LoadAllData<TModel>(
        string sql,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        return await connection.QueryAsync<TModel>(
                new CommandDefinition(sql, cancellationToken: cts)
            ) ?? Enumerable.Empty<TModel>();
    }

    public async Task<IEnumerable<TParent>> LoadAllData<TParent, TChild>(
        string sql,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        var result = await connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(sql, cancellationToken: cts),
            MapNestedObjects
        );

        return result;
    }

    public async Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        return await connection.QueryAsync<TModel>(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<IEnumerable<TParent>> LoadData<TParent, TChild, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        var result = await connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(sql, parameters, cancellationToken: cts),
            MapNestedObjects
        );

        return result;
    }

    public async Task<TModel?> LoadFirst<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        return await connection.QueryFirstOrDefaultAsync<TModel?>(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<TParent?> LoadFirst<TParent, TChild, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        var result = await connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(sql, parameters, cancellationToken: cts),
            MapNestedObjects
        );

        return result.FirstOrDefault();
    }

    public async Task<TResult> SaveData<TParameters, TResult>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        return await connection.QueryFirstAsync<TResult>(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<TResultParent> SaveData<TParameters, TResultParent, TResultChild>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    )
    {
        using IDbConnection connection = new NpgsqlConnection(
            _config.GetConnectionString(connectionId)
        );

        var result = await connection.QueryAsync<TResultParent, TResultChild, TResultParent>(
            new CommandDefinition(sql, parameters, cancellationToken: cts),
            MapNestedObjects
        );

        return result.First();
    }

    private TParent MapNestedObjects<TParent, TChild>(TParent parent, TChild child)
    {
        var parentType = typeof(TParent);
        var childType = typeof(TChild);
        var defaultPropertyName = childType.Name;

        var propertyTypeOfChild = HasAffix
            ? parentType.GetProperty(defaultPropertyName.Subtract(Affix))
            : parentType.GetProperty(defaultPropertyName);

        propertyTypeOfChild?.SetValue(parent, child);

        return parent;
    }
}
