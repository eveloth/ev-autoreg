using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IMapper _mapper;
    private readonly IValidator<UserCredentialsRequest> _validator;

    public AuthController(
        IAuthenticationService authService,
        IValidator<UserCredentialsRequest> validator,
        IMapper mapper
    )
    {
        _authService = authService;
        _validator = validator;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns a JWT and a refresh token
    /// </summary>
    /// <response code="200">Returns a JWT and a refresh token</response>
    /// <response code="400">If credentials are invalid or the user is blocked</response>
    /// <response code="404">If the user doesn't exist</response>
    [AllowAnonymous]
    [Route("token")]
    [HttpPost]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> Login(
        [FromBody] UserCredentialsRequest request,
        CancellationToken cts
    )
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var token = await _authService.Login(request.Email, request.Password, cts);

        var response = _mapper.Map<TokenResponse>(token);

        return Ok(response);
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <response code="200">Returns a JWT and a refresh token</response>
    /// <response code="400">If a validation error occured</response>
    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserCredentialsRequest request,
        CancellationToken cts
    )
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var token = await _authService.Register(request.Email, request.Password, cts);

        var response = _mapper.Map<TokenResponse>(token);
        return Ok(response);
    }

    /// <summary>
    /// Issues a new JWT/refresh token pair
    /// </summary>
    /// <response code="200">Returns a JWT and a refresh token</response>
    /// <response code="400">If a token is invalid</response>
    [AllowAnonymous]
    [Route("refresh")]
    [HttpPost]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cts
    )
    {
        var token = _mapper.Map<Domain.Token>(request);
        var refreshedToken = await _authService.RefreshToken(token, cts);
        var response = _mapper.Map<TokenResponse>(refreshedToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns a list of all user's permissions
    /// </summary>
    /// <response code="200">Returns a list of all user's permissions</response>
    [Authorize]
    [Route("me")]
    [HttpGet]
    [Produces("application/json")]
    public IActionResult GetMe()
    {
        var claims = HttpContext.User.Claims.Where(n => n.Type == "Permission");

        var userPermissions = claims.Select(claim => claim.Type + ": " + claim.Value).ToList();

        var response = new Response<IEnumerable<string>>(userPermissions);
        return Ok(response);
    }
}