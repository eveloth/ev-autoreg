namespace EvAutoreg.Services;

public interface IAuthenticationService
{
    bool IsEmailValid(string email);
    bool IsPasswordValid(string email, string password);
    string GenerateToken(string userId, string roleName);
}