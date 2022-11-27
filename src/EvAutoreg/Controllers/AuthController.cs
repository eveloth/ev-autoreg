using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Requests;
using EvAutoreg.Contracts.Responses;
using EvAutoreg.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationService _authService;

    public AuthController(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthenticationService authService,
        ILogger<AuthController> logger,
        IUnitofWork unitofWork,
        IMapper mapper
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
        _logger = logger;
        _unitofWork = unitofWork;
        _mapper = mapper;
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
    public async Task<IActionResult> Login(
        [FromBody] UserCredentialsRequest request,
        CancellationToken cts
    )
    {
        var existingUser = await _unitofWork.UserRepository.GetUserByEmail(request.Email, cts);

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

        var response = new Response<string>(token);

        return Ok(response);
    }

    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserCredentialsRequest request,
        CancellationToken cts
    )
    {
        if (!_authService.IsEmailValid(request.Email))
        {
            return BadRequest(ErrorCode[1001]);
        }

        if (!_authService.IsPasswordValid(request.Email, request.Password))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var userExists = await _userRepository.DoesUserExist(request.Email, cts);

        if (userExists)
        {
            return BadRequest(ErrorCode[1003]);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var newUser = new UserModel { Email = request.Email, PasswordHash = passwordHash };

        var createdUser = await _unitofWork.UserRepository.CreateUser(newUser, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("User ID {UserId} was registered", createdUser.Id);

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(createdUser));

        return Ok(response);
    }

    [Authorize]
    [Route("me")]
    [HttpGet]
    public IActionResult GetMe()
    {
        var claims = HttpContext.User.Claims.Where(n => n.Type == "Permission");

        var userPermissions = claims.Select(claim => claim.Type + ": " + claim.Value).ToList();

        var response = new Response<IEnumerable<string>>(userPermissions);

        return Ok(response);
    }
}