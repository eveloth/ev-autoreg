namespace EvAutoreg.Data.Repository.Interfaces;

public interface IGeneralPurposeRepository
{
    Task<bool> IsTableEmpty(string tableName, CancellationToken cts);
}