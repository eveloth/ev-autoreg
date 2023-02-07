using Dapper;

namespace EvAutoreg.Data.DataAccess;

public interface ISqlDataAccess
{
    bool HasAffix { get; set; }
    string Affix { get; set; }
    string SplitOn { get; set; }

    Task<IEnumerable<TModel>> LoadAllData<TModel>(string sql, CancellationToken cts);

    Task<IEnumerable<TParent>> LoadAllData<TParent, TChild>(string sql, CancellationToken cts);

    Task<IEnumerable<TModel>> LoadData<TModel>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    );

    Task<IEnumerable<TParent>> LoadData<TParent, TChild>(
        string sql,
        DynamicParameters parameters,
        CancellationToken cts
    );

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
    );

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
    );
}