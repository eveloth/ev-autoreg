﻿using System.Data;
using System.Data.Common;
using Dapper;
using Extensions;

namespace DataAccessLibrary.SqlDataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IDbConnection _connection;
    private readonly DbTransaction _transaction;

    public bool HasAffix { get; set; } = SqlDataAccessOptions.HasAffix;
    public string Affix { get; set; } = SqlDataAccessOptions.Affix;
    public string SplitOn { get; set; } = SqlDataAccessOptions.SplitOn;

    public SqlDataAccess(IDbConnection connection, DbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<TModel>> LoadAllData<TModel>(string sql, CancellationToken cts)
    {
        return await _connection.QueryAsync<TModel>(
                new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction)
            ) ?? Enumerable.Empty<TModel>();
    }

    public async Task<IEnumerable<TParent>> LoadAllData<TParent, TChild>(
        string sql,
        CancellationToken cts
    )
    {
        var result = await _connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction),
            MapNestedObjects,
            SplitOn
        );

        return result;
    }

    public async Task<IEnumerable<TModel>> LoadData<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        return await _connection.QueryAsync<TModel>(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<IEnumerable<TParent>> LoadData<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        var result = await _connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(sql, parameters, cancellationToken: cts),
            MapNestedObjects,
            SplitOn
        );

        return result;
    }

    public async Task<TModel?> LoadFirst<TModel>(string sql, CancellationToken cts)
    {
        return await _connection.QueryFirstOrDefaultAsync<TModel?>(
            new CommandDefinition(sql, cancellationToken: cts)
        );
    }

    public async Task<TModel?> LoadFirst<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        return await _connection.QueryFirstOrDefaultAsync<TModel?>(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<TParent?> LoadFirst<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        var result = await _connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(sql, parameters, cancellationToken: cts),
            MapNestedObjects,
            SplitOn
        );

        return result.FirstOrDefault();
    }

    public async Task SaveData(string sql, DynamicParameters parameters, CancellationToken cts)
    {
        await _connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<TResult> SaveData<TResult>(string sql, CancellationToken cts)
    {
        return await _connection.QueryFirstAsync<TResult>(
            new CommandDefinition(sql, cancellationToken: cts)
        );
    }

    public async Task<TResult> SaveData<TResult>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        return await _connection.QueryFirstAsync<TResult>(
            new CommandDefinition(sql, parameters, cancellationToken: cts)
        );
    }

    public async Task<TResultParent> SaveData<TResultParent, TResultChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        var result = await _connection.QueryAsync<TResultParent, TResultChild, TResultParent>(
            new CommandDefinition(sql, parameters, cancellationToken: cts),
            MapNestedObjects,
            SplitOn
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