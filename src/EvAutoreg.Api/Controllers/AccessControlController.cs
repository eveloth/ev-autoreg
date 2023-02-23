using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Tags("Access control management")]
public class AccessControlController : ControllerBase
{
    private readonly ILogger<AccessControlController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IValidator<RoleRequest> _roleValidator;

    public AccessControlController(
        ILogger<AccessControlController> logger,
        IMapper mapper,
        IRolePermissionService rolePermissionService,
        IPermissionService permissionService,
        IRoleService roleService,
        IUserService userService,
        IValidator<RoleRequest> roleValidator
    )
    {
        _logger = logger;
        _mapper = mapper;
        _rolePermissionService = rolePermissionService;
        _permissionService = permissionService;
        _roleService = roleService;
        _userService = userService;
        _roleValidator = roleValidator;
    }

    /// <summary>
    /// Returns all roles in the system
    /// </summary>
    /// <response code="200">Returns all roles in the system</response>
    [Authorize(Policy = "ReadRoles")]
    [Route("roles")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RoleDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllRoles(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var roles = await _roleService.GetAll(pagination, cts);

        var response = new PagedResponse<RoleDto>(
            _mapper.Map<IEnumerable<RoleDto>>(roles),
            pagination
        );

        return Ok(response);
    }

    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <response code="200">Creates a new role</response>
    /// <response code="400">If a validation error occured</response>
    [Authorize(Policy = "CreateRoles")]
    [Route("roles")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> AddRole([FromBody] RoleRequest request, CancellationToken cts)
    {
        await _roleValidator.ValidateAndThrowAsync(request, cts);

        var role = _mapper.Map<Role>(request);
        var newRole = await _roleService.Add(role, cts);

        _logger.LogInformation(
            "Role ID {RoleId} was added with name {RoleName}",
            newRole.Id,
            newRole.RoleName
        );

        var response = new Response<RoleDto>(_mapper.Map<RoleDto>(newRole));
        return Ok(response);
    }

    /// <summary>
    /// Changes role name
    /// </summary>
    /// <response code="200">Changes role name</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="404">If role doesn't exist</response>
    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{id:int}")]
    [HttpPut]
    [ProducesResponseType(typeof(Response<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> ChangeRoleName(
        [FromRoute] int id,
        [FromBody] RoleRequest request,
        CancellationToken cts
    )
    {
        await _roleValidator.ValidateAndThrowAsync(request, cts);

        var role = new Role { Id = id, RoleName = request.RoleName };
        var updatedRole = await _roleService.Rename(role, cts);

        _logger.LogInformation(
            "Role ID {RoleId} name was changed to {RoleName}",
            updatedRole.Id,
            updatedRole.RoleName
        );

        var response = new Response<RoleDto>(_mapper.Map<RoleDto>(updatedRole));
        return Ok(response);
    }

    /// <summary>
    /// Deletes a role from the system
    /// </summary>
    /// <response code="200">Deletes a role from the system</response>
    /// <response code="404">If role doesn't exist</response>
    [Authorize(Policy = "DeleteRoles")]
    [Route("roles/{id:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteRole([FromRoute] int id, CancellationToken cts)
    {
        var deletedRole = await _roleService.Delete(id, cts);

        _logger.LogInformation(
            "Role ID {RoleId} with name {RoleName} was deleted",
            deletedRole.Id,
            deletedRole.RoleName
        );

        var response = new Response<RoleDto>(_mapper.Map<RoleDto>(deletedRole));
        return Ok(response);
    }

    /// <summary>
    /// Returns all permissions in the system
    /// </summary>
    /// <response code="200">Returns all permissions in the system</response>
    [Authorize(Policy = "ReadPermissions")]
    [Route("permissions")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PermissionDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllPermissions(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var permissions = await _permissionService.GetAll(pagination, cts);

        var response = new PagedResponse<PermissionDto>(
            _mapper.Map<IEnumerable<PermissionDto>>(permissions),
            pagination
        );
        return Ok(response);
    }

    /// <summary>
    /// Returns all role-permission correlations in the system
    /// </summary>
    /// <response code="200">Returns all role-permission correlations in the system</response>
    [Authorize(Policy = "ReadRoles")]
    [Route("roles/permissions")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RolePermissionDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllRolePermissions(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var rolePermissions = await _rolePermissionService.GetAll(pagination, cts);

        var response = new PagedResponse<RolePermissionDto>(
            _mapper.Map<IEnumerable<RolePermissionDto>>(rolePermissions),
            pagination
        );
        return Ok(response);
    }

    /// <summary>
    /// Returns a role permissions correlation for the specified role
    /// </summary>
    /// <response code="200">Returns a role permissions correlation for the specified role</response>
    /// <response code="404">If role or role-permission correlation doesn't exist</response>
    [Authorize(Policy = "ReadRoles")]
    [Route("roles/{id:int}/permissions")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int id, CancellationToken cts)
    {
        var rolePermission = await _rolePermissionService.Get(id, cts);

        var response = _mapper.Map<RolePermissionDto>(rolePermission);
        return Ok(response);
    }

    /// <summary>
    /// Adds a permission to the role
    /// </summary>
    /// <response code="200">Adds a permission to the role</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="400">If a user has insuffitient rights to assign priveleged permissions</response>
    /// <response code="404">If role or permission doesn't exist</response>
    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AddPermissionToRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
        CancellationToken cts
    )
    {
        var rolePermissionrpModel = new RolePermission { Role = new Role { Id = roleId } };
        rolePermissionrpModel.Permissions.Add(new Permission { Id = permissionId });

        var createdCorrelation = await _rolePermissionService.AddPermissionToRole(
            rolePermissionrpModel,
            cts
        );

        _logger.LogInformation(
            "Permission ID {PermissionId} was added to role ID {RoleId}",
            createdCorrelation.Permissions.First().Id,
            createdCorrelation.Role.Id
        );

        var response = new Response<RolePermissionDto>(
            _mapper.Map<RolePermissionDto>(createdCorrelation)
        );
        return Ok(response);
    }

    /// <summary>
    /// Adds a priveleged permission to the role
    /// </summary>
    /// <response code="200">Adds a priveleged permission to the role</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="404">If role or permission doesn't exist</response>
    [Authorize(Policy = "AddPrivelegedPermissionToRole")]
    [Route("roles/{roleId:int}/permissions/priveleged/{permissionId:int}")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AddPrivelegedPermissionToRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
        CancellationToken cts
    )
    {
        var rolePermissionrpModel = new RolePermission { Role = new Role { Id = roleId } };
        rolePermissionrpModel.Permissions.Add(new Permission { Id = permissionId });

        var createdCorrelation = await _rolePermissionService.AddPrivelegedPermissionToRole(
            rolePermissionrpModel,
            cts
        );

        _logger.LogInformation(
            "Priveleged permission ID {PermissionId} was added to role ID {RoleId}",
            createdCorrelation.Permissions.First().Id,
            createdCorrelation.Role.Id
        );

        var response = new Response<RolePermissionDto>(
            _mapper.Map<RolePermissionDto>(createdCorrelation)
        );
        return Ok(response);
    }

    /// <summary>
    /// Removes a permission from role
    /// </summary>
    /// <response code="200">Removes a permission from role</response>
    /// <response code="400">If a user has insuffitient rights to remove priveleged permissions</response>
    /// <response code="404">If role, or permission, or role-permission correlation doesn't exist</response>
    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> RemovePermissionFromRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
        CancellationToken cts
    )
    {
        var rolePermissionModel = new RolePermission { Role = new Role { Id = roleId } };
        rolePermissionModel.Permissions.Add(new Permission { Id = permissionId });

        var deletedCorrelation = await _rolePermissionService.RemovePermissionFromRole(
            rolePermissionModel,
            cts
        );

        _logger.LogInformation(
            "Permission ID {PermissionId} was removed from ID {RoleId}",
            deletedCorrelation.Permissions.First().Id,
            deletedCorrelation.Role.Id
        );

        var response = new Response<RolePermissionDto>(
            _mapper.Map<RolePermissionDto>(deletedCorrelation)
        );
        return Ok(response);
    }

    /// <summary>
    /// Removes a priveleged permission from role
    /// </summary>
    /// <response code="200">Removes a priveleged permission from role</response>
    /// <response code="404">If role, or permission, or role-permission correlation doesn't exist</response>
    [Authorize(Policy = "RemovePrivelegedPermissionFromRole")]
    [Route("roles/{roleId:int}/permissions/priveleged/{permissionId:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> RemovePrivelegedPermissionFromRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
        CancellationToken cts
    )
    {
        var rolePermissionModel = new RolePermission { Role = new Role { Id = roleId } };
        rolePermissionModel.Permissions.Add(new Permission { Id = permissionId });

        var deletedCorrelation = await _rolePermissionService.RemovePrivelegedPermissionFromRole(
            rolePermissionModel,
            cts
        );

        _logger.LogInformation(
            "Permission ID {PermissionId} was removed from ID {RoleId}",
            deletedCorrelation.Permissions.First().Id,
            deletedCorrelation.Role.Id
        );

        var response = new Response<RolePermissionDto>(
            _mapper.Map<RolePermissionDto>(deletedCorrelation)
        );
        return Ok(response);
    }

    /// <summary>
    /// Assignes a role to the specified user
    /// </summary>
    /// <response code="200">Assignes a role to the specified user</response>
    /// <response code="400">If a user has insuffitient rights to assign priveleged role</response>
    /// <response code="404">If user or role doesn't exist</response>
    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{userId:int}/roles/{roleId:int}")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AddUserToRole(
        [FromRoute] int userId,
        [FromRoute] int roleId,
        CancellationToken cts
    )
    {
        var user = new User
        {
            Id = userId,
            Role = new Role { Id = roleId }
        };

        var updatedUser = await _userService.AddUserToRole(user, cts);

        _logger.LogInformation(
            "User ID {UserId} was added to role ID {RoleId}",
            updatedUser.Id,
            updatedUser.Role!.Id
        );

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    /// <summary>
    /// Assignes a priveleged role to the specified user
    /// </summary>
    /// <response code="200">Assignes a priveleged role to the specified user</response>
    /// <response code="404">If user or role doesn't exist</response>
    [Authorize(Policy = "AssignPrivelegedRole")]
    [Route("users/{userId:int}/roles/priveleged/{roleId:int}")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AddUserToPrivelegedRole(
        [FromRoute] int userId,
        [FromRoute] int roleId,
        CancellationToken cts
    )
    {
        var user = new User
        {
            Id = userId,
            Role = new Role { Id = roleId }
        };

        var updatedUser = await _userService.AddUserToPrivelegedRole(user, cts);

        _logger.LogInformation(
            "User ID {UserId} was added to role ID {RoleId}",
            updatedUser.Id,
            updatedUser.Role!.Id
        );

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    /// <summary>
    /// Removes a user from their role
    /// </summary>
    /// <response code="200">Removes a user from their role</response>
    /// <response code="400">If a user has insuffitient rights to remove from priveleged role</response>
    /// <response code="404">If user doesn't exist</response>
    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{id:int}/roles")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> RemoveUserFromRole([FromRoute] int id, CancellationToken cts)
    {
        var updatedUser = await _userService.RemoveUserFromRole(id, cts);

        _logger.LogInformation("User ID {UserId} was removed from their role", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }

    /// <summary>
    /// Removes a user from priveleged role
    /// </summary>
    /// <response code="200">Removes a user from priveleged role</response>
    /// <response code="404">If user doesn't exist</response>
    [Authorize(Policy = "RemoveFromPrivelegedRole")]
    [Route("users/{id:int}/roles/priveleged")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> RemoveUserFromPrivelegedRole(
        [FromRoute] int id,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        if (id == userId)
        {
            throw new ApiException().WithApiError(ErrorCode[2006]);
        }

        var updatedUser = await _userService.RemoveUserFromPrivelegedRole(id, cts);

        _logger.LogInformation("User ID {UserId} was removed from their role", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }
}