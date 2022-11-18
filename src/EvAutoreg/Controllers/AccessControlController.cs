using DataAccessLibrary.DbModels;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var roles = await _unitofWork.RoleRepository.GetRoles(cts);

        await _unitofWork.CommitAsync(cts);

        return Ok(roles);
    }

    [Authorize(Policy = "CreateRoles")]
    [Route("roles")]
    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] RoleDto roleName, CancellationToken cts)
    {
        var newRoleName = roleName.RoleName.ToLower();

        var newRole = await _unitofWork.RoleRepository.AddRole(newRoleName, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Role ID {RoleId} was added with name {RoleName}",
            newRole.Id,
            newRole.RoleName
        );
        return Ok(newRole);
    }

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(
        [FromRoute] int id,
        [FromBody] RoleDto roleName,
        CancellationToken cts
    )
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var role = new RoleModel { Id = id, RoleName = roleName.RoleName.ToLower() };

        var updatedRole = await _unitofWork.RoleRepository.ChangeRoleName(role, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Role ID {RoleId} name was changed to {RoleName}",
            updatedRole.Id,
            updatedRole.RoleName
        );

        return Ok(updatedRole);
    }

    [Authorize(Policy = "DeleteRoles")]
    [Route("roles/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole([FromRoute] int id, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var deletedRole = await _unitofWork.RoleRepository.DeleteRole(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Role ID {RoleId} with name {RoleName} was deleted",
            deletedRole.Id,
            deletedRole.RoleName
        );

        return Ok(deletedRole);
    }

    [Authorize(Policy = "ReadPermissions")]
    [Route("permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cts)
    {
        var permissions = await _unitofWork.PermissionRepository.GetAllPermissions(cts);

        await _unitofWork.CommitAsync(cts);

        return Ok(permissions);
    }

    [Authorize(Policy = "CreatePermissions")]
    [Route("permissions")]
    [HttpPost]
    public async Task<IActionResult> AddPermission(
        [FromBody] PermissionDto permission,
        CancellationToken cts
    )
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

    [Authorize(Policy = "DeletePermissions")]
    [Route("permission/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeletePermission([FromRoute] int id, CancellationToken cts)
    {
        var permissionExists = await _unitofWork.PermissionRepository.DoesPermissionExist(id, cts);

        if (!permissionExists)
        {
            return BadRequest(ErrorCode[4002]);
        }

        var deletedPermission = await _unitofWork.PermissionRepository.DeletePermission(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Permission ID {PermissionId} with name {PermissionName} was deleted",
            deletedPermission.Id,
            deletedPermission.PermissionName
        );

        return Ok(deletedPermission);
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllRolePermissions(CancellationToken cts)
    {
        var rolePermissions = await _unitofWork.RolePermissionRepository.GetAllRolePermissions(cts);

        await _unitofWork.CommitAsync(cts);

        return Ok(rolePermissions);
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/{id:int}/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int id, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesRoleExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var rolePermissions = await _unitofWork.RolePermissionRepository.GetRolePermissions(
            id,
            cts
        );

        await _unitofWork.CommitAsync(cts);

        return Ok(rolePermissions);
    }

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddPermissionToRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
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

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpDelete]
    public async Task<IActionResult> RemovePermissionFromRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
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

        var rolePermissions = await _unitofWork.RolePermissionRepository.DeletePermissionFromRole(
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

    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{userId:int}/roles/{roleId:int}")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(
        [FromRoute] int userId,
        [FromRoute] int roleId,
        CancellationToken cts
    )
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

        var updatedUser = await _unitofWork.RoleRepository.SetUserRole(userId, roleId, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "User ID {UserId} was added to role ID {RoleId}",
            updatedUser.Id,
            updatedUser.Role!.Id
        );

        return Ok(updatedUser);
    }

    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{id:int}/roles")]
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromRole([FromRoute] int id, CancellationToken cts)
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

        var updatedUser = await _unitofWork.RoleRepository.RemoveUserFromRole(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "User ID {UserId} was removed from role ID {RoleId}",
            updatedUser.Id,
            existingUser.Role!.Id
        );

        return Ok(updatedUser);
    }
}
