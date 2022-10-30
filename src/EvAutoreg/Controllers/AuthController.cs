using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _config;

    public AuthController(IUserRepository userRepository, IUserRolesRepository userRolesRepository, IConfiguration config, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _userRolesRepository = userRolesRepository;
        _config = config;
        _passwordHasher = passwordHasher;
    }
        
    [AllowAnonymous]
    [Route("token")]
    [HttpPost]
    public async Task<IActionResult> Login(UserCredentialsDto request, CancellationToken cts)
    {
        var email = request.Email.ToLower();

        var existingUser = await _userRepository.GetUserByEmail(email, cts);

        if (existingUser is null)
        {
            return NotFound("User doesn't exist");
        }

        if (_passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password) !=
            PasswordVerificationResult.Success) 
            return Unauthorized();
            
        var userRole = await _userRolesRepository.GetUserRole(existingUser.Id, cts);
        var role = userRole?.RoleName ?? "Anonymous";
        var issuer = _config["Jwt:Issuer"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new JwtSecurityToken(issuer, claims: claims, expires: DateTime.Now.AddHours(12),
            signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return Ok($"bearer {token}");
    }
       
    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> RegisterUser(UserCredentialsDto request, CancellationToken cts)
    {
        var email = request.Email.ToLower();

        var userExists = await _userRepository.DoesUserExist(email, cts);

        if (userExists) return BadRequest("User already exists.");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var newUser = new UserModel
        {
            Email = email,
            PasswordHash = passwordHash
        };

        await _userRepository.CreateUser(newUser, cts);

        return Ok(newUser.Email);
    }
}