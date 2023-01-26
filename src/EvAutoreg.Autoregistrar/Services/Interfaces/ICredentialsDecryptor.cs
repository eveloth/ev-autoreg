using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface ICredentialsDecryptor
{
    ExtraViewCredentials DecryptEvCredentials(EvCredentialsModel credentials);
    ExchangeCredentials DecryptExchangeCredentials(ExchangeCredentialsModel credentials);
}