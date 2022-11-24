using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts.Extensions;
using EvAutoreg.Exceptions;
using EvAutoreg.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Services;

public class AuthenticationService : IAuthenticationService
{
    // This is, of course, not quite comprehensive check, but we don't send confirmation emails either,
    // so we accept any string that matches the most common email pattern as an email
    private const string EmailValidationRegex = @"[\w\d\-\.\+]+@[\w\d]+\.\w+";

    // Matches a password with minimum 8 characters, at least one uppercase letter, lowercase letter,
    // a number and a special character. Our security standards are highhh....
    private const string PasswordValidationRegex =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\(\)])[A-Za-z\d@$!%*?&\(\)]{8,}$";

    private readonly Regex _emailRegex = new(EmailValidationRegex);
    private readonly Regex _passwordRegex = new(PasswordValidationRegex);

    private readonly IConfiguration _config;
    private readonly IUnitofWork _unitofWork;

    public AuthenticationService(IConfiguration config, IUnitofWork unitofWork)
    {
        _config = config;
        _unitofWork = unitofWork;
    }

    public bool IsEmailValid(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && _emailRegex.IsMatch(email);
    }

    public bool IsPasswordValid(string email, string password)
    {
        return password != email && _passwordRegex.IsMatch(password);
    }

    public async Task<string> GenerateToken(UserModel user, CancellationToken cts)
    {
        var issuer = _config["Jwt:Issuer"] ?? throw new NullConfigurationEntryException();
        var keyString = _config["Jwt:Key"] ?? throw new NullConfigurationEntryException();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()) };

        if (user.RoleId is not null)
        {
            var rolePermissionModels =
                await _unitofWork.RolePermissionRepository.GetRolePermissions(
                    user.RoleId!.Value,
                    cts
                );

            var rolePermissions = rolePermissionModels.ToRolePermissionDto();

            if (rolePermissions.Permissions.Count != 0)
            {
                claims.AddRange(
                    rolePermissions.Permissions.Select(
                        permission => new Claim("Permission", permission.PermissionName!)
                    )
                );
            }
        }

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
