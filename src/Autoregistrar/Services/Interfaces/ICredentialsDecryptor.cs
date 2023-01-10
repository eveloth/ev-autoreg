using Autoregistrar.Domain;
using DataAccessLibrary.Models;

namespace Autoregistrar.Services.Interfaces;

public interface ICredentialsDecryptor
{
    ExtraViewCredentials DecryptEvCredentials(EvCredentialsModel credentials);
    ExchangeCredentials DecryptExchangeCredentials(ExchangeCredentialsModel credentials);
}