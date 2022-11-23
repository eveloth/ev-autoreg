using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Extensions;
using EvAutoreg.Contracts.Requests;
using EvAutoreg.Contracts.Responses;
using EvAutoreg.Services;
using EvAutoreg.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUnitofWork _unitofWork;
    private readonly IAuthenticationService _authService;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(
        ILogger<UsersController> logger,
        IAuthenticationService authService,
        IPasswordHasher passwordHasher,
        IUnitofWork unitofWork
    )
    {
        _logger = logger;
        _authService = authService;
        _passwordHasher = passwordHasher;
        _unitofWork = unitofWork;
    }

    [Authorize(Policy = "ReadUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = pagination.ToFilter();
        var users = await _unitofWork.UserRepository.GetAllUserProfiles(paginationFilter, cts);

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<UserProfileDto>(users.ToUserProfileCollection(), pagination);

        return Ok(response);
    }

    [Authorize]
    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cts)
    {
        var user = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        await _unitofWork.CommitAsync(cts);

        return user is null ? NotFound(ErrorCode[2001]) : Ok(new Response<UserProfileDto>(user.ToUserProfileDto()));
    }

    [Authorize(Policy = "ResetUserPasswords")]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] int id,
        [FromBody] UserPasswordRequest password,
        CancellationToken cts
    )
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

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

        var userWithPassswordReset = await _unitofWork.UserRepository.UpdateUserPassword(
            id,
            passwordHash,
            cts
        );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("Password was reset for user ID {UserId}", userWithPassswordReset);

        var response = new Response<int>(userWithPassswordReset);

        return Ok(response);
    }

    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/block")]
    [HttpPost]
    public async Task<IActionResult> BlockUser([FromRoute] int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        if (existingUser is null || existingUser.IsDeleted)
        {
            return NotFound(ErrorCode[2001]);
        }

        var blockedUser = await _unitofWork.UserRepository.BlockUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was blocked", blockedUser.Id);
        var response = new Response<UserProfileDto>(blockedUser.ToUserProfileDto());

        return Ok(response);
    }

    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/unblock")]
    [HttpPost]
    public async Task<IActionResult> UnblockUser([FromRoute] int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        if (existingUser is null || existingUser.IsDeleted)
        {
            return NotFound(ErrorCode[2001]);
        }

        var unblockedUser = await _unitofWork.UserRepository.UnblockUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was unblocked", unblockedUser.Id);
        var response = new Response<UserProfileDto>(unblockedUser.ToUserProfileDto());

        return Ok(response);
    }

    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromRoute] int id, CancellationToken cts)
    {
        var userExists = await _unitofWork.UserRepository.DoesUserExist(id, cts);

        if (!userExists)
        {
            return NotFound("User not found");
        }

        var deletedUser = await _unitofWork.UserRepository.DeleteUser(id, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("User ID {UserId} was deleted", deletedUser.Id);
        
        var response = new Response<UserProfileDto>(deletedUser.ToUserProfileDto());

        return Ok(response);
    }

    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}/restore")]
    [HttpPost]
    public async Task<IActionResult> RestoreUser([FromRoute] int id, CancellationToken cts)
    {
        var userExists = await _unitofWork.UserRepository.DoesUserExist(id, cts);

        if (!userExists)
        {
            return NotFound("User not found");
        }

        var restoredUser = await _unitofWork.UserRepository.RestoreUser(id, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("User ID {UserId} was restored", restoredUser.Id);
        
        var response = new Response<UserProfileDto>(restoredUser.ToUserProfileDto());

        return Ok(response);
    }
}