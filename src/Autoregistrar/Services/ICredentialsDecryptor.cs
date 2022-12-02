using Autoregistrar.Contracts.Dto;
using DataAccessLibrary.Models;

namespace Autoregistrar.Services;

public interface ICredentialsDecryptor
{
    EvCredentialsDto DecryptEvCredentials(EvCredentialsModel credentials);
    ExchangeCredentialsDto DecryptExchangeCredentials(ExchangeCredentialsModel credentials);
}