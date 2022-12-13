using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
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

    [Authorize(Policy = "ReadUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var users = await _userService.GetAll(pagination, cts);

        var response = new PagedResponse<UserDto>(
            _mapper.Map<IEnumerable<UserDto>>(users),
            pagination
        );

        return Ok(response);
    }

    [Authorize]
    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cts)
    {
        var user = await _userService.Get(id, cts);

        return Ok(_mapper.Map<UserDto>(user));
    }

    [Authorize(Policy = "ResetUserPasswords")]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] int id,
        [FromBody] UserPasswordRequest request,
        CancellationToken cts
    )
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var updatedUserId = await _userService.ChangePassword(id, request.NewPassword, cts);

        _logger.LogInformation("Password was reset for user ID {UserId}", updatedUserId);

        var response = new ResultResponse(true);
        return Ok(response);
    }

    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/block")]
    [HttpPost]
    public async Task<IActionResult> BlockUser([FromRoute] int id, CancellationToken cts)
    {
        var updatedUser = await _userService.Block(id, cts);

        _logger.LogInformation("User ID {UserId} was blocked", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/unblock")]
    [HttpPost]
    public async Task<IActionResult> UnblockUser([FromRoute] int id, CancellationToken cts)
    {
        var updatedUser = await _userService.Unblock(id, cts);

        _logger.LogInformation("User ID {UserId} was unblocked", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromRoute] int id, CancellationToken cts)
    {
        var updatedUser = await _userService.Delete(id, cts);

        _logger.LogInformation("User ID {UserId} was deleted", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}/restore")]
    [HttpPost]
    public async Task<IActionResult> RestoreUser([FromRoute] int id, CancellationToken cts)
    {
        var updatedUser = await _userService.Restore(id, cts);

        _logger.LogInformation("User ID {UserId} was restored", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }
}