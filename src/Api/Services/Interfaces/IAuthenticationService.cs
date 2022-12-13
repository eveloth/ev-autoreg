using Api.Domain;
using DataAccessLibrary.Models;

namespace Api.Services.Interfaces;

public interface IAuthenticationService
{
    Task<Token> Register(string email, string password, CancellationToken cts);
    Task<Token> Login(string email, string password, CancellationToken cts);
    Task<Token> RefreshToken(RefreshToken token, CancellationToken cts);
}