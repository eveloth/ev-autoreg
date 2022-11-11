using DataAccessLibrary.DisplayModels;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[AllowAnonymous]
[Route("api/access-control")]
[ApiController]
public class AccessControlController : ControllerBase
{
    private readonly ILogger<AccessControlController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAccessControlRepository _acRepository;

    public AccessControlController(
        IAccessControlRepository acRepository,
        ILogger<AccessControlController> logger,
        IUserRepository userRepository
    )
    {
        _acRepository = acRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    [Route("roles")]
    [HttpGet]
    public async Task<IActionResult> GetAllRoles(CancellationToken cts)
    {
        try
        {
            var roles = await _acRepository.GetRoles(cts);
            return Ok(roles);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
        
    [Route("roles")]
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleDto roleName, CancellationToken cts)
    {
        var newRoleName = roleName.RoleName.ToLower();

        try
        {
            var newRole = await _acRepository.AddRole(newRoleName, cts);
            _logger.LogInformation(
                "Role ID {RoleId} was added with name {RoleName}",
                newRole.Id,
                newRole.RoleName
            );
            return Ok(newRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
        
    [Route("roles/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(int id, RoleDto roleName, CancellationToken cts)
    {
        var roleExists = await _acRepository.DoesRoleExist(id, cts);
            
        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }
            
        var role = new RoleModel
        {
            Id = id,
            RoleName = roleName.RoleName.ToLower()
        };

        try
        {
            var updatedRole = await _acRepository.ChangeRoleName(role, cts);
            _logger.LogInformation(
                "Role ID {RoleId} name was changed to {RoleName}",
                updatedRole.Id,
                updatedRole.RoleName
            );
            return Ok(updatedRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("roles/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cts)
    {
        var roleExists = await _acRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }
            
        try
        {
            var deletedRole = await _acRepository.DeleteRole(id, cts);
            _logger.LogInformation(
                "Role ID {RoleId} with name {RoleName} was deleted",
                deletedRole.Id,
                deletedRole.RoleName
            );
            return Ok(deletedRole);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cts)
    {
        try
        {
            var permissions = await _acRepository.GetAllPermissions(cts);
            return Ok(permissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("permissions")]
    [HttpPost]
    public async Task<IActionResult> AddPermission(PermissionDto permission, CancellationToken cts)
    {
        var permissionExists = await _acRepository.DoesPermissionExist(permission.PermissionName, cts);

        if (permissionExists)
        {
            return BadRequest(ErrorCode[4001]);
        }
        
        var newPermission = new PermissionModel
        {
            PermissionName = permission.PermissionName,
            Description = permission.Description
        };

        try
        {
            var addedPermission = await _acRepository.AddPermission(newPermission, cts);
            _logger.LogInformation(
                "Permission ID {PermissionId} was added with name {PermissionName}",
                addedPermission.Id,
                addedPermission.PermissionName
            );
            return Ok(addedPermission);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("permission/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeletePermission(int id, CancellationToken cts)
    {
        var permissionExists = await _acRepository.DoesPermissionExist(id, cts);

        if (!permissionExists)
        {
            return BadRequest(ErrorCode[4002]);
        }
            
        try
        {
            var deletedPermission = await _acRepository.DeletePermission(id, cts);
            _logger.LogInformation(
                "Permission ID {PermissionId} with name {PermissionName} was deleted",
                deletedPermission.Id,
                deletedPermission.PermissionName
            );
            return Ok(deletedPermission);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
        
    [Route("roles/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllRolePermissions(CancellationToken cts)
    {
        try
        {
            var rolePermissions = await _acRepository.GetAllRolePermissions(cts);
            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
        
    [Route("roles/{id:int}/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetRolePermissions(int id, CancellationToken cts)
    {
        var roleExists = await _acRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }
            
        try
        {
            var rolePermissions = await _acRepository.GetRolePermissions(id, cts);
            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddPermissionToRole(int roleId, int permissionId, CancellationToken cts)
    {
        var roleExists = await _acRepository.DoesRoleExist(roleId, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }
        
        var permissionExists = await _acRepository.DoesPermissionExist(permissionId, cts);

        if (!permissionExists)
        {
            return BadRequest(ErrorCode[4002]);
        }

        try
        {
            var rolePermissions = await _acRepository.AddPermissionToRole(roleId, permissionId, cts);
            _logger.LogInformation("Permission ID {PermissionId} was added to role ID {RoleId}", permissionId, roleId);
            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
    
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpDelete]
    public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId, CancellationToken cts)
    {
        var rolePermissionCorerlationExists =
            await _acRepository.DoesRolePermissionCorrecationExist(roleId, permissionId, cts);

        if (!rolePermissionCorerlationExists)
        {
            return BadRequest(ErrorCode);
        }
        
        try
        {
            var rolePermissions = await _acRepository.DeletePermissionFromRole(roleId, permissionId, cts);
            _logger.LogInformation("Permission ID {PermissionId} was removed from ID {RoleId}", permissionId, roleId);
            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("users/{userId:int}/roles/{roleId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(int userId, int roleId, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(userId, cts);

        if (!userExists)
        {
            return NotFound(ErrorCode[2001]);
        }

        var roleExists = await _acRepository.DoesRoleExist(roleId, cts);

        if (!roleExists)
        {
            return NotFound(ErrorCode[3001]);
        }

        try
        {
            var updatedUser = await _acRepository.SetUserRole(userId, roleId, cts);
                
            _logger.LogInformation(
                "User ID {UserId} was added to role ID {RoleId}",
                updatedUser.Id,
                updatedUser.Role!.Id
            );
                
            return Ok(updatedUser);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
    
    [Route("users/{id:int}/roles")]
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromRole(int id, CancellationToken cts)
    {
        var existingUser = await _userRepository.GetUserProfle(id, cts);

        if (existingUser is null)
        {
            return NotFound(ErrorCode[2001]);
        }

        var isInAnyRole = existingUser.Role is not null;

        if (!isInAnyRole)
        {
            return BadRequest(ErrorCode[3002]);
        }

        try
        {
            var updatedUser = await _acRepository.RemoveUserFromRole(id, cts);
                
            _logger.LogInformation(
                "User ID {UserId} was removed from role ID {RoleId}",
                updatedUser.Id,
                existingUser.Role!.Id
            );
                
            return Ok(updatedUser);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
}