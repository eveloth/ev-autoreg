using DataAccessLibrary.Models;

namespace DataAccessLibrary.SqlDataAccess;

public interface ISqlDataAccess
{
    Task<IEnumerable<TModel>> LoadAllData<TModel>(
        string sql,
        string connectionId = "Default");

    Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string sql,
        TParameters parameters,
        string connectionId = "Default");

    Task SaveData<TParameters>(
        string sql,
        TParameters parameters,
        string connectionId = "Default");
}