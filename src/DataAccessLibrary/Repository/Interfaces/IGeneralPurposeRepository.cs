namespace DataAccessLibrary.Repository;

public interface IGeneralPurposeRepository
{
    Task<bool> IsTableEmpty(string tableName, CancellationToken cts);
}