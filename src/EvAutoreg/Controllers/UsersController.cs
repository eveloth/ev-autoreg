using System.Security.Claims;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    //[Authorize(Roles = "manager, admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken cts)
    {
        try
        {
            var users = await _userRepository.GetAllUserProfiles(cts);
            return Ok(users);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUser(int id, CancellationToken cts)
    {
        try
        {
            var user = await _userRepository.GetUserProfle(id, cts);
            return user is null ? NotFound(ErrorCode[2001]) : Ok(user);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("profile")]
    [HttpPatch]
    public async Task<IActionResult> UpdateUserProfile(
        UserProfileDto profile,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        try
        {
            var user = await _userRepository.UpdateUserProfile(
                userId,
                profile.FisrtName,
                profile.LastName,
                cts
            );
            _logger.LogInformation("User profile was updated for user ID {UserId}", userId);
            return Ok(user);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}/block")]
    [HttpPost]
    public async Task<IActionResult> BlockUser(int id, CancellationToken cts)
    {
        var existingUser = await _userRepository.GetUserProfle(id, cts);

        if (existingUser is null || existingUser.IsDeleted) return NotFound(ErrorCode[2001]);

        try
        {
            var userId = await _userRepository.BlockUser(id, cts);
            _logger.LogInformation("User ID {UserId} was blocked", userId);
            return Ok(userId);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}/unblock")]
    [HttpPost]
    public async Task<IActionResult> UnblockUser(int id, CancellationToken cts)
    {
        var existingUser = await _userRepository.GetUserProfle(id, cts);

        if (existingUser is null || existingUser.IsDeleted) return NotFound(ErrorCode[2001]);

        try
        {
            var userId = await _userRepository.UnblockUser(id, cts);
            _logger.LogInformation("User ID {UserId} was unblocked", userId);
            return Ok(userId);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists)
            return NotFound("User not found");

        try
        {
            var user = await _userRepository.DeleteUser(id, cts);
            _logger.LogInformation("User ID {UserId} was deleted", user.Id);
            return Ok(user);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [AllowAnonymous]
    [Route("fancynewuser/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserWithRole(int id, CancellationToken cts)
    {
        return Ok(await _userRepository.GetNewUserModel(id, cts));
    }
}