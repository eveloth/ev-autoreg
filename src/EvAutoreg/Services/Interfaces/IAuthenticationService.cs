using DataAccessLibrary.Models;

namespace EvAutoreg.Services.Interfaces;

public interface IAuthenticationService
{
    bool IsEmailValid(string email);
    bool IsPasswordValid(string email, string password);
    Task<string> GenerateToken(UserModel user, CancellationToken cts);
}
