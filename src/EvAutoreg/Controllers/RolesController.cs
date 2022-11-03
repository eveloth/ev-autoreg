using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly ILogger<RolesController> _logger;
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IUserRepository _userRepository;

    public RolesController(IUserRolesRepository userRolesRepository, IUserRepository userRepository, ILogger<RolesController> logger)
    {
        _userRolesRepository = userRolesRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [Authorize(Roles="admin")]
    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cts)
    {
        try
        {
            var roles = await _userRolesRepository.GetRoles(cts);
            return Ok(roles);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles="admin")]
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleDto role, CancellationToken cts)
    {
        var roleName = role.RoleName.ToLower();

        try
        {
            var newRole = await _userRolesRepository.AddRole(roleName, cts);
            _logger.LogInformation("Role ID {RoleId} was added with name {RoleName}", newRole.Id, newRole.RoleName);
            return Ok(newRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(int id, RoleDto role, CancellationToken cts)
    {
        var newRoleName = role.RoleName.ToLower();

        try
        {
            var newRole = await _userRolesRepository.ChangeRoleName(id, newRoleName, cts);
            _logger.LogInformation("Role ID {RoleId} name was changed to {RoleName}", newRole.Id, newRole.RoleName);
            return Ok(newRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    /// <summary>
    /// Deletes the role and removes that role from all the users associated with it.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cts)
    {
        try
        {
            var deletedRole = await _userRolesRepository.DeleteRole(id, cts);
            _logger.LogInformation("Role ID {RoleId} with name {RoleName} was deleted", deletedRole.Id, deletedRole.RoleName);
            return Ok(deletedRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpGet]
    public async Task<IActionResult> GetAllUserRoles(CancellationToken cts)
    {
        try
        {
            var userRoles = await _userRolesRepository.GetAllUserRoles(cts);
            return Ok(userRoles);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserRole(int id, CancellationToken cts)
    {
        try
        {
            var userRole = await _userRolesRepository.GetUserRole(id, cts);
            return Ok(userRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    } 

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(UserRoleDto userRole, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(userRole.UserId, cts);

        if (!userExists)
        {
            return NotFound(ErrorCode[2001]);
        }

        var roleExists = await _userRolesRepository.DoesRoleExist(userRole.RoleId, cts);

        if (!roleExists)
        {
            return NotFound(ErrorCode[3001]);
        }

        try
        {
            var newUserRole = await _userRolesRepository.SetUserRole(userRole.UserId, userRole.RoleId, cts);
            _logger.LogInformation("User ID {UserId} was added to role ID {RoleId}", newUserRole.UserId, newUserRole.RoleId);
            return Ok(newUserRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUserFromRole(UserRoleDto userRole, CancellationToken cts)
    {
        var recordExists = await _userRolesRepository.DoesRecordExist(userRole.UserId, userRole.RoleId, cts);

        if (!recordExists) return NotFound("No user with the specified role found.");

        try
        {
            var deletedUserRole = await _userRolesRepository.DeleteUserFromRole(userRole.UserId, userRole.RoleId, cts);
            _logger.LogInformation("User ID {UserId} was removed from role ID {RoleId}", deletedUserRole.UserId, deletedUserRole.RoleId);
            return Ok(deletedUserRole);
        }
        catch (NpgsqlException e)
        {
           _logger.LogError("{ErrorMessage}", e.Message);
           return StatusCode(500, ErrorCode[9001]);
        }
    }
}