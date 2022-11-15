using System.Security.Claims;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Repository;
using DataAccessLibrary.Repository.Interfaces;
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
    private readonly IUnitofWork _unitofWork;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationService _authService;

    public AuthController(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthenticationService authService,
        ILogger<AuthController> logger,
        IUnitofWork unitofWork
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
        _logger = logger;
        _unitofWork = unitofWork;
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

        var existingUser = await _unitofWork.UserRepository.GetUserByEmail(email, cts);

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

        var token = await _authService.GenerateToken(existingUser, cts);

        await _unitofWork.CommitAsync(cts);

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
            var createdUser = await _unitofWork.UserRepository.CreateUser(newUser, cts);
            await _unitofWork.CommitAsync(cts);
            _logger.LogInformation("User ID {UserId} was registered", createdUser.Id);
            return Ok(createdUser);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize]
    [Route("me")]
    [HttpGet]
    public IActionResult GetMe()
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var claims = HttpContext.User.Claims.Where(n => n.Type == "Permission");

        var clst = claims.Select(claim => claim.Type + ": " + claim.Value).ToList();

        return Ok(clst);
    }

}
