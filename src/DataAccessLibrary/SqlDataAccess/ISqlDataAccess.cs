using DataAccessLibrary.Models;

namespace DataAccessLibrary.SqlDataAccess;

public interface ISqlDataAccess
{
    Task<IEnumerable<TModel>> LoadAllData<TModel>(
        string sql,
        CancellationToken cts,
        string connectionId = "Default");

    Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default");

    Task<TModel?> LoadFirst<TModel, TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default"
    );

    Task SaveData<TParameters>(
        string sql,
        TParameters parameters,
        CancellationToken cts,
        string connectionId = "Default");
}