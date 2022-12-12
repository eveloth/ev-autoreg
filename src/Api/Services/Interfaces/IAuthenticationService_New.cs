using Api.Domain;
using DataAccessLibrary.Models;

namespace Api.Services.Interfaces;

public interface IAuthenticationService_New
{
    Task<AuthenticationResult> Register(UserModel user, CancellationToken cts);
    Task<AuthenticationResult> Login(UserModel user, string password, CancellationToken cts);
    Task<AuthenticationResult> RefreshToken(RefreshToken token, CancellationToken cts);
}