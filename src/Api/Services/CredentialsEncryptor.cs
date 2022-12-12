using System.Security.Cryptography;
using System.Text;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Domain;
using Api.Exceptions;
using Api.Services.Interfaces;
using DataAccessLibrary.Models;

namespace Api.Services;

public class CredentialsEncryptor : ICredentialsEncryptor
{
    private readonly IConfiguration _config;

    public CredentialsEncryptor(IConfiguration config)
    {
        _config = config;
    }

    public EvCredentialsModel EncryptEvCredentials(int userId, EvCredentials credentials)
    {
        var encrypfedCredentials = new EvCredentialsModel();
        var key =
            _config["SymmetricSecurityKey"]
            ?? throw new NullConfigurationEntryException(
                "Couldn't fetch SymmetricSecurityKey from configuraion, check entry."
            );

        using var aes = Aes.Create();

        encrypfedCredentials.UserId = userId;
        encrypfedCredentials.IV = aes.IV;

        aes.Key = Encoding.ASCII.GetBytes(key);
        aes.Padding = PaddingMode.PKCS7;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        encrypfedCredentials.EncryptedEmail = Encrypt(credentials.Email, encryptor);
        encrypfedCredentials.EncryptedPassword = Encrypt(credentials.Password, encryptor);

        return encrypfedCredentials;
    }

    public ExchangeCredentialsModel EncryptExchangeCredentials(
        int userId,
        ExchangeCredentials credentials
    )
    {
        var encrypfedCredentials = new ExchangeCredentialsModel();
        var key =
            _config["SymmetricSecurityKey"]
            ?? throw new NullConfigurationEntryException(
                "Couldn't fetch SymmetricSecurityKey from configuraion, check entry."
            );

        using var aes = Aes.Create();

        encrypfedCredentials.UserId = userId;
        encrypfedCredentials.IV = aes.IV;

        aes.Key = Encoding.ASCII.GetBytes(key);
        aes.Padding = PaddingMode.PKCS7;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        encrypfedCredentials.EncryptedEmail = Encrypt(credentials.Email, encryptor);
        encrypfedCredentials.EncryptedPassword = Encrypt(credentials.Password, encryptor);

        return encrypfedCredentials;
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

    private static byte[] Encrypt(string valueToEncrypt, ICryptoTransform encryptor)
    {
        byte[] encrypted;

        using (var memStream = new MemoryStream())
        {
            using (var cryptoSream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            {
                using (var streamWriter = new StreamWriter(cryptoSream))
                {
                    streamWriter.Write(valueToEncrypt);
                }
                encrypted = memStream.ToArray();
            }
        }

        return encrypted;
    }

    private static string Decrypt(byte[] valueToDecrypt, ICryptoTransform decryptor)
    {
        using var memStream = new MemoryStream(valueToDecrypt);
        using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
}