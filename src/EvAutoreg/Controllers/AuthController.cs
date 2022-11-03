using System.Security.Claims;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationService _authService;

    public AuthController(
        IUserRepository userRepository,
        IUserRolesRepository userRolesRepository,
        IPasswordHasher passwordHasher,
        IAuthenticationService authService,
        ILogger<AuthController> logger
    )
    {
        _userRepository = userRepository;
        _userRolesRepository = userRolesRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
        _logger = logger;
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

        if (
            _passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password)
            != PasswordVerificationResult.Success
        )
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

        _logger.LogInformation("User ID {UserId} was logged in", existingUser.Id);

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

        var newUser = new UserModel { Email = email, PasswordHash = passwordHash };

        try
        {
            var createdUser = await _userRepository.CreateUser(newUser, cts);
            _logger.LogInformation("User ID {UserId} was registered", createdUser.Id);
            return Ok(createdUser);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("user/email")]
    [HttpPatch]
    public async Task<IActionResult> UpdateEmail(UserEmailDto email, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var newEmail = email.NewEmail.ToLower();

        if (!_authService.IsEmailValid(newEmail))
        {
            return BadRequest(ErrorCode[1001]);
        }

        try
        {
            var userProfile = await _userRepository.UpdateUserEmail(userId, newEmail, cts);
            _logger.LogInformation(
                "Email was updated to {NewEmail} for user ID {UserId}",
                userProfile.Email,
                userProfile.Id
            );
            return Ok(userProfile);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("user/password")]
    [HttpPatch]
    public async Task<IActionResult> UpdatePassword(UserPasswordDto password, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var user = await _userRepository.GetUserById(userId, cts);
        var email = user!.Email;

        if (!_authService.IsPasswordValid(email, password.NewPassword))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        try
        {
            var userWithChangedPassword = await _userRepository.UpdateUserPassword(
                userId,
                passwordHash,
                cts
            );
            _logger.LogInformation(
                "Password was changed for user ID {UserId}",
                userWithChangedPassword
            );
            return Ok(userWithChangedPassword);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("user/{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        int id,
        UserPasswordDto password,
        CancellationToken cts
    )
    {
        var existingUser = await _userRepository.GetUserProfle(id, cts);

        if (existingUser is null)
        {
            return NotFound(ErrorCode[2001]);
        }

        var email = existingUser.Email.ToLower();

        if (!_authService.IsPasswordValid(email, password.NewPassword))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        try
        {
            var userWithPassswordReset = await _userRepository.UpdateUserPassword(
                id,
                passwordHash,
                cts
            );
            _logger.LogInformation(
                "Password was reset for user ID {UserId}",
                userWithPassswordReset
            );
            return Ok(userWithPassswordReset);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
}
