using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Tags("Current user management")]
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

    /// <summary>
    /// Returns a profile of the current user
    /// </summary>
    /// <response code="200">Returns a profile of the current user</response>
    [HttpGet]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        var me = await _userService.Get(userId, cts);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(me));
        return Ok(response);
    }

    /// <summary>
    /// Updates an email of the current user
    /// </summary>
    /// <response code="200">Updates an email of the current user</response>
    /// <response code="400">If a validation error occured</response>
    [Route("email")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateEmail(
        [FromBody] UserEmailRequest request,
        CancellationToken cts
    )
    {
        await _emailValidator.ValidateAndThrowAsync(request, cts);

        var userId = HttpContext.GetUserId();

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

    /// <summary>
    /// Updates a password of the current user
    /// </summary>
    /// <response code="200">Updates a password of the current user</response>
    /// <response code="400">If a validation error occured</response>
    [Route("password")]
    [HttpPost]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> UpdatePassword(
        [FromBody] UserPasswordRequest request,
        CancellationToken cts
    )
    {
        await _passwordValidator.ValidateAndThrowAsync(request, cts);

        var userId = HttpContext.GetUserId();

        var updatedUserId = await _userService.ChangePassword(userId, request.NewPassword, cts);

        _logger.LogInformation("Password was changed for user ID {UserId}", updatedUserId);

        var response = new SuccessResponse(true);
        return Ok(response);
    }

    /// <summary>
    /// Updates the first and the last name of the current user
    /// </summary>
    /// <response code="200">Updates the first and the last name of the current user</response>
    /// <response code="400">If a validation error occured</response>
    [HttpPatch]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateUserProfile(
        [FromBody] UserProfileRequest request,
        CancellationToken cts
    )
    {
        await _profileValidator.ValidateAndThrowAsync(request, cts);

        var userId = HttpContext.GetUserId();

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