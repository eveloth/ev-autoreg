namespace Data.SqlDataAccess;

public interface ISqlDataAccess
{
    Task<IEnumerable<TModel>> LoadAll<TModel>(
        string procedure,
        string connectionId = "Default");

    Task<IEnumerable<TModel>> LoadData<TModel, TParameters>(
        string procedure,
        TParameters parameters,
        string connectionId = "Default");

    Task SaveData<TParameters>(
        string procedure,
        TParameters parameters,
        string connectionId = "Default");
}