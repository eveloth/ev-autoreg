using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Services.Interfaces;
using DataAccessLibrary.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Api.Errors.ErrorCodes;
using Token = Api.Domain.Token;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IValidator<UserCredentialsRequest> _validator;

    public AuthController(
        IAuthenticationService authService,
        IValidator<UserCredentialsRequest> validator
    )
    {
        _authService = authService;
        _validator = validator;
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
        await _validator.ValidateAndThrowAsync(request, cts);

        var token = await _authService.Login(request.Email, request.Password, cts);

        var response = new Response<Token>(token);

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
        await _validator.ValidateAndThrowAsync(request, cts);

        var token = await _authService.Register(request.Email, request.Password, cts);

        var response = new Response<Token>(token);

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