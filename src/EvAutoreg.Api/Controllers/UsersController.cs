using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Cache;
using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Queries;
using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Tags("User management")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IValidator<UserPasswordRequest> _validator;

    public UsersController(
        ILogger<UsersController> logger,
        IMapper mapper,
        IUserService userService,
        IValidator<UserPasswordRequest> validator
    )
    {
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
        _validator = validator;
    }

    /// <summary>
    /// Returns all users in the system
    /// </summary>
    /// <response code="200">Returns all users in the system</response>
    [Cached(300)]
    [Authorize(Policy = "ReadUsers")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var users = await _userService.GetAll(pagination, cts);
        var usersCount = await _userService.Count(cts);

        var response = new PagedResponse<UserDto>(
            _mapper.Map<IEnumerable<UserDto>>(users),
            pagination,
            usersCount
        );

        return Ok(response);
    }

    /// <summary>
    /// Returns a scecified user
    /// </summary>
    /// <response code="200">Returns a scecified user</response>
    /// <response code="404">If a user doesn't exist</response>
    [Cached(300)]
    [Authorize(Policy = "ReadUsers")]
    [Route("{id:int}")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cts)
    {
        var user = await _userService.Get(id, cts);

        return Ok(_mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Resets a password for the specified user to the provided one
    /// </summary>
    /// <response code="200">Resets a password for the specified user to the provided one</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="404">If a user doesn't exist</response>
    [Authorize(Policy = "ResetUserPasswords")]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] int id,
        [FromBody] UserPasswordRequest request,
        CancellationToken cts
    )
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var updatedUserId = await _userService.ChangePassword(id, request.NewPassword, cts);

        _logger.LogInformation("Password was reset for user ID {UserId}", updatedUserId);

        return Ok();
    }

    /// <summary>
    /// Blocks a user
    /// </summary>
    /// <response code="200">Blocks a user</response>
    /// <response code="400">If a user tries to block themselves or if a user is in priveleged role</response>
    /// <response code="404">If a user doesn't exist</response>
    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/block")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> BlockUser([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        if (userId == id)
        {
            throw new ApiException().WithApiError(ErrorCode[1013]);
        }

        var updatedUser = await _userService.Block(id, cts);

        _logger.LogInformation("User ID {UserId} was blocked", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    /// <summary>
    /// Unblocks a user
    /// </summary>
    /// <response code="200">Unblocks a user</response>
    /// <response code="400">If a user tries to unblock themselves</response>
    /// <response code="404">If a user doesn't exist</response>
    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/unblock")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> UnblockUser([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        if (userId == id)
        {
            throw new ApiException().WithApiError(ErrorCode[1013]);
        }

        var updatedUser = await _userService.Unblock(id, cts);

        _logger.LogInformation("User ID {UserId} was unblocked", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <response code="200">Unblocks a user</response>
    /// <response code="400">If a user tries to delete themselves or if a user is in priveleged role</response>
    /// <response code="404">If a user doesn't exist</response>
    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        if (userId == id)
        {
            throw new ApiException().WithApiError(ErrorCode[1013]);
        }

        var updatedUser = await _userService.Delete(id, cts);

        _logger.LogInformation("User ID {UserId} was deleted", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    /// <summary>
    /// Restores a user
    /// </summary>
    /// <response code="200">Restores a user</response>
    /// <response code="400">If a user tries to restore themselves</response>
    /// <response code="404">If a user doesn't exist</response>
    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}/restore")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> RestoreUser([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        if (userId == id)
        {
            throw new ApiException().WithApiError(ErrorCode[1013]);
        }

        var updatedUser = await _userService.Restore(id, cts);

        _logger.LogInformation("User ID {UserId} was restored", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }
}