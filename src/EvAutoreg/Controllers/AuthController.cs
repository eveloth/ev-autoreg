using DataAccessLibrary.Models;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationService _authService;

    public AuthController(IUserRepository userRepository, IUserRolesRepository userRolesRepository, IConfiguration config, IPasswordHasher passwordHasher, IAuthenticationService authService)
    {
        _userRepository = userRepository;
        _userRolesRepository = userRolesRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }
    
    /// <summary>
    /// Logs user in.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cts"></param>
    /// <returns>A JWT token to include in HTTP request header or an error object with the error context</returns>
    /// <response code="200">Returns JWT token</response>
    /// <response code="401">If credentials are invalid</response>
    /// <response code="400">If user is blocked</response>
    /// <response code="404">If user doesn't exist</response>
    [AllowAnonymous]
    [Route("token")]
    [HttpPost]
    public async Task<IActionResult> Login(UserCredentialsDto request, CancellationToken cts)
    {
        var email = request.Email.ToLower();

        var existingUser = await _userRepository.GetUserByEmail(email, cts);

        if (existingUser is null)
        {
            return NotFound(ErrorCode[1004]);
        }

        if (_passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password) !=
            PasswordVerificationResult.Success)
        {
            return Unauthorized(ErrorCode[1005]);
        }

        if (existingUser.IsBlocked)
        {
            return BadRequest(ErrorCode[1006]);
        }
        
        var userRole = await _userRolesRepository.GetUserRole(existingUser.Id, cts);
        
        var role = userRole?.RoleName ?? "anonymous";
        var id = existingUser.Id.ToString();

        var token = _authService.GenerateToken(id, role);

        return Ok(token);
    }
       
    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> RegisterUser(UserCredentialsDto request, CancellationToken cts)
    {
        var email = request.Email.ToLower();
        
        if (!_authService.IsEmailValid(email))
        {
            return BadRequest(ErrorCode[1001]);
        }

        if (!_authService.IsPasswordValid(email, request.Password))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var userExists = await _userRepository.DoesUserExist(email, cts);

        if (userExists)
        {
            return BadRequest(ErrorCode[1003]);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var newUser = new UserModel
        {
            Email = email,
            PasswordHash = passwordHash
        };

        try
        {
            await _userRepository.CreateUser(newUser, cts);
            return Ok(newUser.Email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
}