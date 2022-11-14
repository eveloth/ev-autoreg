namespace DataAccessLibrary.SqlDataAccess;

public interface ISqlDataAccess
{
    bool HasAffix { get; set; }
    string Affix { get; set; }
    string SplitOn { get; set; }

    Task<IEnumerable<TModel>> LoadAllData<TModel>(
        string sql,
        CancellationToken cts
    );

    Task<IEnumerable<TParent>> LoadAllData<TParent, TChild>(
        string sql,
        CancellationToken cts
    );

    Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts
    );

    Task<IEnumerable<TParent>> LoadData<TParent, TChild, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts
    );

    Task<TModel?> LoadFirst<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts
    );

    Task<TParent?> LoadFirst<TParent, TChild, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts
    );

    Task<TResult> SaveData<TResult>(
        string sql,
        CancellationToken cts
    );

    Task<TResult> SaveData<TParameters, TResult>(
        string sql,
        TParameters parameters,
        CancellationToken cts
    );

    Task<TResultParent> SaveData<TParameters, TResultParent, TResultChild>(
        string sql,
        TParameters parameters,
        CancellationToken cts
    );
}
