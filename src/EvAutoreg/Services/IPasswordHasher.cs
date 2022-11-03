using Microsoft.AspNetCore.Identity;

namespace EvAutoreg.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    PasswordVerificationResult VerifyPassword(string hash, string password);
}
