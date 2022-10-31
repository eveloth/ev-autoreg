using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Services;

public class AuthenticationService : IAuthenticationService
{
    // This is, of course, not quite comprehensive check, but we don't send confirmation emails either,
    // so we accept any string that matches the most common email pattern as an email
    private const string _emailValidationRegex = @"[\w\d\-\.\+]+@[\w\d]+.\w+";
    
    // Matches a password with minimum 8 characters, at least one uppercase letter, lowercase letter,
    // a number and a special character. Our security standards are highhh....
    private const string _passwordValidationRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\(\)])[A-Za-z\d@$!%*?&\(\)]{8,}$";

    private readonly Regex _emailRegex = new Regex(_emailValidationRegex);
    private readonly Regex _passwordRegex = new Regex(_passwordValidationRegex);

    private readonly IConfiguration _config;
    public AuthenticationService(IConfiguration config)
    {
        _config = config;
    }

    public bool IsEmailValid(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && _emailRegex.IsMatch(email);
    }
    
    public bool IsPasswordValid(string email, string password)
    {
        return password != email && _passwordRegex.IsMatch(password);
    }
    
    public string GenerateToken(string userId, string roleName)
    {
        var issuer = _config["Jwt:Issuer"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, roleName)
        };
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var tokenDescriptor = new JwtSecurityToken(issuer, claims: claims, expires: DateTime.Now.AddHours(12),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}