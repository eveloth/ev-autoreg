using System.Data;
using System.Data.Common;
using Dapper;
using EvAutoreg.Extensions;
using Microsoft.Extensions.Logging;

namespace EvAutoreg.Data.DataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IDbConnection _connection;
    private readonly DbTransaction _transaction;
    private const string List = "List";
    private const string Add = "Add";
    private const string PluralSuffix = "s";

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
        where TParent : class
        where TChild : class
    {
        return await _connection.QueryAsync<TParent, TChild, TParent>(
                new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction),
                MapNestedObjects,
                SplitOn
            ) ?? Enumerable.Empty<TParent>();
    }

    public async Task<IEnumerable<TParent>> LoadAllData<TParent, TChild1, TChild2>(
        string sql,
        CancellationToken cts
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class
    {
        return await _connection.QueryAsync<TParent, TChild1, TChild2, TParent>(
                new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction),
                MapNestedObjects,
                SplitOn
            ) ?? Enumerable.Empty<TParent>();
    }

    public async Task<IEnumerable<TModel>> LoadData<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        return await _connection.QueryAsync<TModel>(
                new CommandDefinition(
                    sql,
                    parameters,
                    cancellationToken: cts,
                    transaction: _transaction
                )
            ) ?? Enumerable.Empty<TModel>();
    }

    public async Task<IEnumerable<TParent>> LoadData<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild : class
    {
        return await _connection.QueryAsync<TParent, TChild, TParent>(
                new CommandDefinition(
                    sql,
                    parameters,
                    cancellationToken: cts,
                    transaction: _transaction
                ),
                MapNestedObjects,
                SplitOn
            ) ?? Enumerable.Empty<TParent>();
    }

    public async Task<IEnumerable<TParent>> LoadData<TParent, TChild1, TChild2>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class
    {
        return await _connection.QueryAsync<TParent, TChild1, TChild2, TParent>(
                new CommandDefinition(
                    sql,
                    parameters,
                    cancellationToken: cts,
                    transaction: _transaction
                ),
                MapNestedObjects,
                SplitOn
            ) ?? Enumerable.Empty<TParent>();
    }

    public async Task<TModel?> LoadSingle<TModel>(string sql, CancellationToken cts)
    {
        return await _connection.QuerySingleOrDefaultAsync<TModel?>(
            new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction)
        );
    }

    public async Task<TModel?> LoadSingle<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        return await _connection.QuerySingleOrDefaultAsync<TModel?>(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            )
        );
    }

    public async Task<TParent?> LoadSingle<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild : class
    {
        var result = await _connection.QueryAsync<TParent, TChild, TParent>(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            ),
            MapNestedObjects,
            SplitOn
        );

        return result.SingleOrDefault();
    }

    public async Task<TParent?> LoadSingle<TParent, TChild1, TChild2>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class
    {
        var result = await _connection.QueryAsync<TParent, TChild1, TChild2, TParent>(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            ),
            MapNestedObjects,
            SplitOn
        );

        return result.SingleOrDefault();
    }

    public async Task<TResult> LoadScalar<TResult>(string sql, CancellationToken cts)
    {
        return await _connection.ExecuteScalarAsync<TResult>(
            new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction)
        );
    }

    public async Task SaveData(string sql, DynamicParameters parameters, CancellationToken cts)
    {
        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            )
        );
    }

    public async Task<TResult> SaveData<TResult>(string sql, CancellationToken cts)
    {
        return await _connection.QuerySingleAsync<TResult>(
            new CommandDefinition(sql, cancellationToken: cts, transaction: _transaction)
        );
    }

    public async Task<TResult> SaveData<TResult>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
    {
        return await _connection.QuerySingleAsync<TResult>(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            )
        );
    }

    public async Task<TResultParent> SaveData<TResultParent, TResultChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TResultParent : class
        where TResultChild : class
    {
        var result = await _connection.QueryAsync<TResultParent, TResultChild, TResultParent>(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            ),
            MapNestedObjects,
            SplitOn
        );

        return result.First();
    }

    public async Task<TResultParent> SaveData<TResultParent, TResultChild1, TResultChild2>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TResultParent : class
        where TResultChild1 : class
        where TResultChild2 : class
    {
        var result = await _connection.QueryAsync<
            TResultParent,
            TResultChild1,
            TResultChild2,
            TResultParent
        >(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cts,
                transaction: _transaction
            ),
            MapNestedObjects,
            SplitOn
        );

        return result.First();
    }

    private TParent MapNestedObjects<TParent, TChild>(TParent parent, TChild child)
        where TParent : class
        where TChild : class
    {
        SetChildType(parent, child);
        return parent;
    }

    private TParent MapNestedObjects<TParent, TChild1, TChild2>(
        TParent parent,
        TChild1 firstChild,
        TChild2 secondChild
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class
    {
        SetChildType(parent, firstChild);
        SetChildType(parent, secondChild);
        return parent;
    }

    private void SetChildType<TParent, TChild>(TParent parent, TChild child)
        where TParent : class
        where TChild : class
    {
        var parentType = typeof(TParent);
        var childType = typeof(TChild);
        string defaultPropertyName;

        if (childType.IsGenericTypeDefinition && IsList(childType))
        {
            defaultPropertyName = childType.GetGenericArguments().Single().Name;

            var collectionTypeOfChild = HasAffix
                ? parentType.GetProperty(defaultPropertyName.Subtract(Affix) + PluralSuffix)
                : parentType.GetProperty(defaultPropertyName + PluralSuffix);

            collectionTypeOfChild?.PropertyType
                .GetMethod(Add)
                ?.Invoke(collectionTypeOfChild.GetValue(parent), new[] { child });
        }
        else
        {
            defaultPropertyName = childType.Name;

            var propertyTypeOfChild = HasAffix
                ? parentType.GetProperty(defaultPropertyName.Subtract(Affix))
                : parentType.GetProperty(defaultPropertyName);

            propertyTypeOfChild?.SetValue(parent, child);
        }
    }

    private static bool IsList(Type type)
    {
        return type.Name[..^2] == List;
    }
}