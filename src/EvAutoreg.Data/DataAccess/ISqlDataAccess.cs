using Dapper;

namespace EvAutoreg.Data.DataAccess;

public interface ISqlDataAccess
{
    bool HasAffix { get; set; }
    string Affix { get; set; }
    string SplitOn { get; set; }

    Task<IEnumerable<TModel>> LoadAllData<TModel>(string sql, CancellationToken cts);

    Task<IEnumerable<TParent>> LoadAllData<TParent, TChild>(string sql, CancellationToken cts)
        where TParent : class
        where TChild : class;
    Task<IEnumerable<TParent>> LoadAllData<TParent, TChild1, TChild2>(
        string sql,
        CancellationToken cts
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class;

    Task<IEnumerable<TModel>> LoadData<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    );

    Task<IEnumerable<TParent>> LoadData<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild : class;

    Task<IEnumerable<TParent>> LoadData<TParent, TChild1, TChild2>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class;

    Task<TModel?> LoadSingle<TModel>(string sql, CancellationToken cts);

    Task<TModel?> LoadSingle<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    );

    Task<TParent?> LoadSingle<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild : class;

    Task<TParent?> LoadSingle<TParent, TChild1, TChild2>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TParent : class
        where TChild1 : class
        where TChild2 : class;

    Task<TResult> LoadScalar<TResult>(string sql, CancellationToken cts);

    Task SaveData(string sql, DynamicParameters parameters, CancellationToken cts);
    Task<TResult> SaveData<TResult>(string sql, CancellationToken cts);

    Task<TResult> SaveData<TResult>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    );

    Task<TResultParent> SaveData<TResultParent, TResultChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TResultParent : class
        where TResultChild : class;

    Task<TResultParent> SaveData<TResultParent, TResultChild1, TResultChild2>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    )
        where TResultParent : class
        where TResultChild1 : class
        where TResultChild2 : class;
}