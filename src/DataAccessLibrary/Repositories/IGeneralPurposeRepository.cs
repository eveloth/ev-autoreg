namespace DataAccessLibrary.Repositories;

public interface IGeneralPurposeRepository
{
    Task<bool> IsTableEmpty(string tableName, CancellationToken cts);
}