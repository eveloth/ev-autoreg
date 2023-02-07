using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IExtCredentialsService
{
    Task<int> SaveExchangeCredentials(int id, ExchangeCredentials credentials, CancellationToken cts);
    Task<int> SaveEvCredentials(int id, EvCredentials credentials, CancellationToken cts);
}