using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Services;

public class AuthenticationService : IAuthenticationService
{
    // This is, of course, not quite comprehensive check, but we don't send confirmation emails either,
    // so we accept any string that matches the most common email pattern as an email
    private const string _emailValidationRegex = @"[\w\d\-\.\+]+@[\w\d]+\.\w+";

    // Matches a password with minimum 8 characters, at least one uppercase letter, lowercase letter,
    // a number and a special character. Our security standards are highhh....
    private const string _passwordValidationRegex =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\(\)])[A-Za-z\d@$!%*?&\(\)]{8,}$";

    private readonly Regex _emailRegex = new(_emailValidationRegex);
    private readonly Regex _passwordRegex = new(_passwordValidationRegex);

    private readonly IConfiguration _config;
    private readonly IUnitofWork _unitofWork;

    public AuthenticationService(
        IConfiguration config,
        IUnitofWork unitofWork
    )
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

    public async Task<string> GenerateToken(User user, CancellationToken cts)
    {
        var issuer = _config["Jwt:Issuer"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()) };

        if (user.RoleId is not null)
        {
            var rolePermissions = await _unitofWork.RolePermissionRepository.GetRolePermissions(
                user.RoleId!.Value,
                cts
            );

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
