using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Domain;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Api.Services.Interfaces;

public interface ICredentialsEncryptor
{
    EvCredentialsModel EncryptEvCredentials(int userId, EvCredentials credentials);
    ExchangeCredentialsModel EncryptExchangeCredentials(
        int userId,
        ExchangeCredentials credentials
    );
}