using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;

namespace DataAccessLibrary.Repository;

public class ExtCredentialsRepository : IExtCredentialsRepository
{
    public Task<EvCredentialsModel> GetEvCredentials(int userId, CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveEvCredentials(EvCredentialsModel evCredentials, CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeCredentialsModel> GetExchangeCredentials(int userId, CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveExchangeCredentials(ExchangeCredentialsModel exchangeCredentials, CancellationToken cts)
    {
        throw new NotImplementedException();
    }
}