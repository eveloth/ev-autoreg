using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IExtCredentialsRepository
{
    Task<EvCredentialsModel?> GetEvCredentials(int userId, CancellationToken cts);
    Task<int> SaveEvCredentials(EvCredentialsModel evCredentials, CancellationToken cts);
    Task<ExchangeCredentialsModel?> GetExchangeCredentials(int userId, CancellationToken cts);
    Task<int> SaveExchangeCredentials(ExchangeCredentialsModel exchangeCredentials, CancellationToken cts);
}