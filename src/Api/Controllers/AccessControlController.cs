using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Api.Errors.ErrorCodes;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessControlController : ControllerBase
{
    private readonly ILogger<AccessControlController> _logger;
    private readonly IUnitofWork _unitofWork;
    private readonly IMapper _mapper;

    public AccessControlController(
        ILogger<AccessControlController> logger,
        IUnitofWork unitofWork,
        IMapper mapper
    )
    {
        _logger = logger;
        _unitofWork = unitofWork;
        _mapper = mapper;
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles")]
    [HttpGet]
    public async Task<IActionResult> GetAllRoles(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);
        var roles = await _unitofWork.RoleRepository.GetAll(paginationFilter, cts);

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<RoleDto>(
            _mapper.Map<IEnumerable<RoleDto>>(roles),
            pagination
        );

        return Ok(response);
    }

    [Authorize(Policy = "CreateRoles")]
    [Route("roles")]
    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] RoleRequest roleName, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesExist(roleName.RoleName, cts);

        if (roleExists)
        {
            return BadRequest(ErrorCode[3003]);
        }
        var newRole = await _unitofWork.RoleRepository.Add(roleName.RoleName, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Role ID {RoleId} was added with name {RoleName}",
            newRole.Id,
            newRole.RoleName
        );

        var roleDto = _mapper.Map<RoleDto>(newRole);

        var response = new Response<RoleDto>(roleDto);

        return Ok(response);
    }

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(
        [FromRoute] int id,
        [FromBody] RoleRequest request,
        CancellationToken cts
    )
    {
        var roleExists = await _unitofWork.RoleRepository.DoesExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var roleNameIsTaken = await _unitofWork.RoleRepository.DoesExist(request.RoleName, cts);

        if (roleNameIsTaken)
        {
            return BadRequest(ErrorCode[3003]);
        }

        var role = new RoleModel { Id = id, RoleName = request.RoleName.ToLower() };

        var updatedRole = await _unitofWork.RoleRepository.ChangeName(role, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Role ID {RoleId} name was changed to {RoleName}",
            updatedRole.Id,
            updatedRole.RoleName
        );

        var response = new Response<RoleDto>(_mapper.Map<RoleDto>(updatedRole));

        return Ok(response);
    }

    [Authorize(Policy = "DeleteRoles")]
    [Route("roles/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole([FromRoute] int id, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var deletedRole = await _unitofWork.RoleRepository.Delete(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Role ID {RoleId} with name {RoleName} was deleted",
            deletedRole.Id,
            deletedRole.RoleName
        );

        var response = new Response<RoleDto>(_mapper.Map<RoleDto>(deletedRole));

        return Ok(response);
    }

    [Authorize(Policy = "ReadPermissions")]
    [Route("permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var permissions = await _unitofWork.PermissionRepository.GetAll(paginationFilter, cts);

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<PermissionDto>(
            _mapper.Map<IEnumerable<PermissionDto>>(permissions),
            pagination
        );

        return Ok(response);
    }

    [Authorize(Policy = "CreatePermissions")]
    [Route("permissions")]
    [HttpPost]
    public async Task<IActionResult> AddPermission(
        [FromBody] PermissionRequest request,
        CancellationToken cts
    )
    {
        var permissionExists = await _unitofWork.PermissionRepository.DoesExist(
            request.PermissionName,
            cts
        );

        if (permissionExists)
        {
            return BadRequest(ErrorCode[4001]);
        }

        var newPermission = new PermissionModel
        {
            PermissionName = request.PermissionName,
            Description = request.Description
        };

        var addedPermission = await _unitofWork.PermissionRepository.Add(newPermission, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Permission ID {PermissionId} was added with name {PermissionName}",
            addedPermission.Id,
            addedPermission.PermissionName
        );

        var response = new Response<PermissionDto>(_mapper.Map<PermissionDto>(addedPermission));

        return Ok(response);
    }

    [Authorize(Policy = "DeletePermissions")]
    [Route("permission/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeletePermission([FromRoute] int id, CancellationToken cts)
    {
        var permissionExists = await _unitofWork.PermissionRepository.DoesExist(id, cts);

        if (!permissionExists)
        {
            return BadRequest(ErrorCode[4002]);
        }

        var deletedPermission = await _unitofWork.PermissionRepository.Delete(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Permission ID {PermissionId} with name {PermissionName} was deleted",
            deletedPermission.Id,
            deletedPermission.PermissionName
        );

        var response = new Response<PermissionDto>(_mapper.Map<PermissionDto>(deletedPermission));

        return Ok(response);
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetAllRolePermissions(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var rolePermissions = await _unitofWork.RolePermissionRepository.GetAll(
            paginationFilter,
            cts
        );

        var listsOfRolePermissions = rolePermissions.GroupByIntoList(
            x => new { x.RoleId, x.RoleName }
        );

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<RolePermissionDto>(
            _mapper.Map<IEnumerable<RolePermissionDto>>(listsOfRolePermissions),
            pagination
        );

        return Ok(response);
    }

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/{id:int}/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int id, CancellationToken cts)
    {
        var roleExists = await _unitofWork.RoleRepository.DoesExist(id, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var rolePermissions = await _unitofWork.RolePermissionRepository.GetRole(id, cts);

        await _unitofWork.CommitAsync(cts);

        var response = _mapper.Map<RolePermissionDto>(rolePermissions.ToList());

        //var response = new Response<RolePermissionDto>(rolePermissions.ToRolePermissionDto());

        return Ok(response);
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
        var roleExists = await _unitofWork.RoleRepository.DoesExist(roleId, cts);

        if (!roleExists)
        {
            return BadRequest(ErrorCode[3001]);
        }

        var permissionExists = await _unitofWork.PermissionRepository.DoesExist(permissionId, cts);

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

        var response = new Response<RolePermissionDto>(
            _mapper.Map<RolePermissionDto>(rolePermissions)
        );

        return Ok(response);
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
            await _unitofWork.RolePermissionRepository.DoesCorrecationExist(
                roleId,
                permissionId,
                cts
            );

        if (!rolePermissionCorerlationExists)
        {
            return BadRequest(ErrorCode);
        }

        var rolePermissions = await _unitofWork.RolePermissionRepository.RemovePermissionFromRole(
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

        var response = new Response<RolePermissionDto>(
            _mapper.Map<RolePermissionDto>(rolePermissions)
        );

        return Ok(response);
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
        var userExists = await _unitofWork.UserRepository.DoesExist(userId, cts);

        if (!userExists)
        {
            return NotFound(ErrorCode[2001]);
        }

        var roleExists = await _unitofWork.RoleRepository.DoesExist(roleId, cts);

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

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(updatedUser));

        return Ok(response);
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

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(updatedUser));

        return Ok(response);
    }
}