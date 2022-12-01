using Api.Contracts.Dto;
using Api.Contracts.Requests;
using DataAccessLibrary.Models;

namespace Api.Services.Interfaces;

public interface ICredentialsEncryptor
{
    EvCredentialsModel EncryptEvCredentials(int userId, EvCredentialsRequest credentials);
    ExchangeCredentialsModel EncryptExchangeCredentials(
        int userId,
        ExchangeCredentialsRequest credentials
    );
    EvCredentialsDto DecryptEvCredentials(EvCredentialsModel credentials);
    ExchangeCredentialsDto DecryptExchangeCredentials(ExchangeCredentialsModel credentials);
}
