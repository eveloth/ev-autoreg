using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Domain;
using DataAccessLibrary.Models;

namespace Api.Services.Interfaces;

public interface ICredentialsEncryptor
{
    EvCredentialsModel EncryptEvCredentials(int userId, EvCredentials credentials);
    ExchangeCredentialsModel EncryptExchangeCredentials(
        int userId,
        ExchangeCredentials credentials
    );
    EvCredentialsDto DecryptEvCredentials(EvCredentialsModel credentials);
    ExchangeCredentialsDto DecryptExchangeCredentials(ExchangeCredentialsModel credentials);
}