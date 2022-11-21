using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts;
using EvAutoreg.Contracts.Extensions;
using EvAutoreg.Dto;
using EvAutoreg.Services;
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

        var response = new PagedResponse<UserProfile>(users, pagination);

        return Ok(response);
    }

    [Authorize]
    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cts)
    {
        var user = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        await _unitofWork.CommitAsync(cts);

        return user is null ? NotFound(ErrorCode[2001]) : Ok(user);
    }

    [Authorize(Policy = "ResetUserPasswords")]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] int id,
        [FromBody] UserPasswordDto password,
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

        var userId = new { UserId = userWithPassswordReset };

        _logger.LogInformation("Password was reset for user ID {UserId}", userId.UserId);

        return Ok(userId);
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

        return Ok(blockedUser);
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

        var blockedUser = await _unitofWork.UserRepository.UnblockUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was unblocked", blockedUser.Id);

        return Ok(blockedUser);
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

        var user = await _unitofWork.UserRepository.DeleteUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was deleted", user.Id);

        return Ok(user);
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

        var user = await _unitofWork.UserRepository.RestoreUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was restored", user.Id);

        return Ok(user);
    }
}
