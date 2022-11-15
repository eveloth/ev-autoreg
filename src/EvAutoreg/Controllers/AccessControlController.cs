using DataAccessLibrary.DbModels;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/access-control")]
[ApiController]
public class AccessControlController : ControllerBase
{
    private readonly ILogger<AccessControlController> _logger;
    private readonly IUnitofWork _unitofWork;

    public AccessControlController(ILogger<AccessControlController> logger, IUnitofWork unitofWork)
    {
        _logger = logger;
        _unitofWork = unitofWork;
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles")]
    [HttpGet]
    public async Task<IActionResult> GetAllRoles(CancellationToken cts)
    {
        try
        {
            var roles = await _unitofWork.RoleRepository.GetRoles(cts);

            await _unitofWork.CommitAsync(cts);

            return Ok(roles);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Policy = "CreateRoles")]
    [Route("roles")]
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleDto roleName, CancellationToken cts)
    {
        var newRoleName = roleName.RoleName.ToLower();

        try
        {
            var newRole = await _unitofWork.RoleRepository.AddRole(newRoleName, cts);

            await _unitofWork.CommitAsync(cts);

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

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(int id, RoleDto roleName, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var role = new RoleModel { Id = id, RoleName = roleName.RoleName.ToLower() };

        try
        {
            var updatedRole = await _unitofWork.RoleRepository.ChangeRoleName(role, cts);

            await _unitofWork.CommitAsync(cts);

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

    [Authorize(Policy = "DeleteRoles")]
    [Route("roles/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        try
        {
            var deletedRole = await _unitofWork.RoleRepository.DeleteRole(id, cts);

            await _unitofWork.CommitAsync(cts);

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

    [Authorize(Policy = "ReadPermissions")]
    [Route("permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cts)
    {
        try
        {
            var permissions = await _unitofWork.PermissionRepository.GetAllPermissions(cts);

            await _unitofWork.CommitAsync(cts);

            return Ok(permissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Policy = "CreatePermissions")]
    [Route("permissions")]
    [HttpPost]
    public async Task<IActionResult> AddPermission(PermissionDto permission, CancellationToken cts)
    {
        var permissionExists = await _unitofWork.PermissionRepository.DoesPermissionExist(
            permission.PermissionName,
            cts
        );

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
            var addedPermission = await _unitofWork.PermissionRepository.AddPermission(
                newPermission,
                cts
            );

            await _unitofWork.CommitAsync(cts);

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

    [Authorize(Policy = "DeletePermissions")]
    [Route("permission/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeletePermission(int id, CancellationToken cts)
    {
        var permissionExists = await _unitofWork.PermissionRepository.DoesPermissionExist(id, cts);

        if (!permissionExists)
        {
            return BadRequest(ErrorCode[4002]);
        }

        try
        {
            var deletedPermission = await _unitofWork.PermissionRepository.DeletePermission(
                id,
                cts
            );

            await _unitofWork.CommitAsync(cts);

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

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllRolePermissions(CancellationToken cts)
    {
        try
        {
            var rolePermissions = await _unitofWork.RolePermissionRepository.GetAllRolePermissions(
                cts
            );

            await _unitofWork.CommitAsync(cts);

            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/{id:int}/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetRolePermissions(int id, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        try
        {
            var rolePermissions = await _unitofWork.RolePermissionRepository.GetRolePermissions(
                id,
                cts
            );

            await _unitofWork.CommitAsync(cts);

            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddPermissionToRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    )
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(roleId, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var permissionExists = await _unitofWork.PermissionRepository.DoesPermissionExist(
            permissionId,
            cts
        );

        if (!permissionExists)
        {
            return BadRequest(ErrorCode[4002]);
        }

        try
        {
            var rolePermissions = await _unitofWork.RolePermissionRepository.AddPermissionToRole(
                roleId,
                permissionId,
                cts
            );

            await _unitofWork.CommitAsync(cts);

            _logger.LogInformation(
                "Permission ID {PermissionId} was added to role ID {RoleId}",
                permissionId,
                roleId
            );
            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpDelete]
    public async Task<IActionResult> RemovePermissionFromRole(
        int roleId,
        int permissionId,
        CancellationToken cts
    )
    {
        var rolePermissionCorerlationExists =
            await _unitofWork.RolePermissionRepository.DoesRolePermissionCorrecationExist(
                roleId,
                permissionId,
                cts
            );

        if (!rolePermissionCorerlationExists)
        {
            return BadRequest(ErrorCode);
        }

        try
        {
            var rolePermissions =
                await _unitofWork.RolePermissionRepository.DeletePermissionFromRole(
                    roleId,
                    permissionId,
                    cts
                );

            await _unitofWork.CommitAsync(cts);

            _logger.LogInformation(
                "Permission ID {PermissionId} was removed from ID {RoleId}",
                permissionId,
                roleId
            );
            return Ok(rolePermissions);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{userId:int}/roles/{roleId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(int userId, int roleId, CancellationToken cts)
    {
        var userExists = await _unitofWork.UserRepository.DoesUserExist(userId, cts);

        if (!userExists)
        {
            return NotFound(ErrorCode[2001]);
        }

        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(roleId, cts);

        if (!roleExists)
        {
            return NotFound(ErrorCode[3001]);
        }

        try
        {
            var updatedUser = await _unitofWork.RoleRepository.SetUserRole(userId, roleId, cts);

            await _unitofWork.CommitAsync(cts);

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

    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{id:int}/roles")]
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromRole(int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

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
            var updatedUser = await _unitofWork.RoleRepository.RemoveUserFromRole(id, cts);

            await _unitofWork.CommitAsync(cts);

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
