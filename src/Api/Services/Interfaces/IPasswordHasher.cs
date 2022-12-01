using Microsoft.AspNetCore.Identity;

namespace Api.Services.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    PasswordVerificationResult VerifyPassword(string hash, string password);
}
