using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain;
using Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MeController : ControllerBase
{
    private readonly ILogger<MeController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IValidator<UserEmailRequest> _emailValidator;
    private readonly IValidator<UserPasswordRequest> _passwordValidator;
    private readonly IValidator<UserProfileRequest> _profileValidator;

    public MeController(
        ILogger<MeController> logger,
        IMapper mapper,
        IUserService userService,
        IValidator<UserEmailRequest> emailValidator,
        IValidator<UserPasswordRequest> passwordValidator,
        IValidator<UserProfileRequest> profileValidator
    )
    {
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
        _emailValidator = emailValidator;
        _passwordValidator = passwordValidator;
        _profileValidator = profileValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!
                .Value
        );

        var me = await _userService.Get(userId, cts);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(me));
        return Ok(response);
    }

    [Route("email")]
    [HttpPost]
    public async Task<IActionResult> UpdateEmail(
        [FromBody] UserEmailRequest request,
        CancellationToken cts
    )
    {
        await _emailValidator.ValidateAndThrowAsync(request, cts);

        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var user = new User { Id = userId, Email = request.Email };

        var updatedUser = await _userService.ChangeEmail(user, cts);

        _logger.LogInformation(
            "Email was updated to {Email} for user ID {UserId}",
            updatedUser.Email,
            updatedUser.Id
        );

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    [Route("password")]
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(
        [FromBody] UserPasswordRequest request,
        CancellationToken cts
    )
    {
        await _passwordValidator.ValidateAndThrowAsync(request, cts);

        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var updatedUserId = await _userService.ChangePassword(userId, request.NewPassword, cts);

        _logger.LogInformation("Password was changed for user ID {UserId}", updatedUserId);

        var response = new ResultResponse(true);
        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateUserProfile(
        [FromBody] UserProfileRequest request,
        CancellationToken cts
    )
    {
        await _profileValidator.ValidateAndThrowAsync(request, cts);

        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var user = new User
        {
            Id = userId,
            FirstName = request.FisrtName,
            LastName = request.LastName
        };

        var updatedUser = await _userService.UpdateProfile(user, cts);

        _logger.LogInformation("User profile was updated for user ID {UserId}", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }
}