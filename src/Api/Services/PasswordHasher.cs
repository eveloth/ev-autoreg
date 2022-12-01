using Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Api.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public PasswordVerificationResult VerifyPassword(string hash, string password)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentNullException(nameof(hash));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        var isValid = BCrypt.Net.BCrypt.Verify(password, hash);

        return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}
