using DataAccessLibrary.Models;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Requests;

namespace EvAutoreg.Services.Interfaces;

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
