using Api.Domain;

namespace Api.Services.Interfaces;

public interface IExtCredentialsService
{
    Task<int> SaveExchangeCredentials(ExchangeCredentials credentials, CancellationToken cts);
    Task<int> SaveEvCredentials(EvCredentials credentials, CancellationToken cts);
}