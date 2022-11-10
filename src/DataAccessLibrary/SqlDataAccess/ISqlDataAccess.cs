namespace DataAccessLibrary.SqlDataAccess;

public interface ISqlDataAccess
{
    bool HasAffix { get; set; }
    string Affix { get; set; }

    Task<IEnumerable<TModel>> LoadAllData<TModel>(
        string sql,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<IEnumerable<TParent>> LoadAllData<TParent, TChild>(
        string sql,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<IEnumerable<TParent>> LoadData<TParent, TChild, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<TModel?> LoadFirst<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<TParent?> LoadFirst<TParent, TChild, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<TResult> SaveData<TParameters, TResult>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task<TResultParent> SaveData<TParameters, TResultParent, TResultChild>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );
}
