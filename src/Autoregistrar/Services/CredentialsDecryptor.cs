using System.Security.Cryptography;
using System.Text;
using Autoregistrar.Contracts.Dto;
using Autoregistrar.Exceptions;
using DataAccessLibrary.Models;

namespace Autoregistrar.Services;

public class CredentialsDecryptor : ICredentialsDecryptor
{
    private readonly IConfiguration _config;

    public CredentialsDecryptor(IConfiguration config)
    {
        _config = config;
    }

    public EvCredentialsDto DecryptEvCredentials(EvCredentialsModel credentials)
    {
        var key =
            _config["SymmetricSecurityKey"]
            ?? throw new NullConfigurationEntryException(
                "Couldn't fetch SymmetricSecurityKey from configuraion, check entry."
            );

        using var aes = Aes.Create();

        aes.Key = Encoding.ASCII.GetBytes(key);
        aes.IV = credentials.IV;
        aes.Padding = PaddingMode.PKCS7;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        var decryptedCredentials = new EvCredentialsDto
        {
            Email = Decrypt(credentials.EncryptedEmail, decryptor),
            Password = Decrypt(credentials.EncryptedPassword, decryptor)
        };

        return decryptedCredentials;
    }

    public ExchangeCredentialsDto DecryptExchangeCredentials(ExchangeCredentialsModel credentials)
    {
        var key =
            _config["SymmetricSecurityKey"]
            ?? throw new NullConfigurationEntryException(
                "Couldn't fetch SymmetricSecurityKey from configuraion, check entry."
            );

        using var aes = Aes.Create();

        aes.Key = Encoding.ASCII.GetBytes(key);
        aes.IV = credentials.IV;
        aes.Padding = PaddingMode.PKCS7;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        var decryptedCredentials = new ExchangeCredentialsDto
        {
            Email = Decrypt(credentials.EncryptedEmail, decryptor),
            Password = Decrypt(credentials.EncryptedPassword, decryptor)
        };

        return decryptedCredentials;
    }

    private static string Decrypt(byte[] valueToDecrypt, ICryptoTransform decryptor)
    {
        string decrypted;

        using (var memStream = new MemoryStream(valueToDecrypt))
        {
            using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
            {
                using (var streamReader = new StreamReader(cryptoStream))
                {
                    decrypted = streamReader.ReadToEnd();
                }
            }
        }

        return decrypted;
    }
}