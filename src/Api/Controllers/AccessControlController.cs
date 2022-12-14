using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain;
using Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
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

    [Authorize(Policy = "ReadRoles")]
    [Route("roles")]
    [HttpGet]
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

    [Authorize(Policy = "CreateRoles")]
    [Route("roles")]
    [HttpPost]
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

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{id:int}")]
    [HttpPut]
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

    [Authorize(Policy = "DeleteRoles")]
    [Route("roles/{id:int}")]
    [HttpDelete]
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

    [Authorize(Policy = "ReadPermissions")]
    [Route("permissions")]
    [HttpGet]
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

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/permissions")]
    [HttpGet]
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

    [Authorize(Policy = "ReadRoles")]
    [Route("roles/{id:int}/permissions")]
    [HttpGet]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int id, CancellationToken cts)
    {
        var rolePermission = await _rolePermissionService.Get(id, cts);

        var response = _mapper.Map<RolePermissionDto>(rolePermission);
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

    [Authorize(Policy = "UpdateRoles")]
    [Route("roles/{roleId:int}/permissions/{permissionId:int}")]
    [HttpDelete]
    public async Task<IActionResult> RemovePermissionFromRole(
        [FromRoute] int roleId,
        [FromRoute] int permissionId,
        CancellationToken cts
    )
    {
        var rolePermissionrpModel = new RolePermission { Role = new Role { Id = roleId } };
        rolePermissionrpModel.Permissions.Add(new Permission { Id = permissionId });

        var deletedCorrelation = await _rolePermissionService.RemovePermissionFromRole(
            rolePermissionrpModel,
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

    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{userId:int}/roles/{roleId:int}")]
    [HttpPost]
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

    [Authorize(Policy = "UpdateUsers")]
    [Route("users/{id:int}/roles")]
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromRole([FromRoute] int id, CancellationToken cts)
    {
        var updatedUser = await _userService.RemoveUserFromRole(id, cts);

        _logger.LogInformation("User ID {UserId} was removed from their role", updatedUser.Id);

        var response = new Response<UserDto>(_mapper.Map<UserDto>(updatedUser));
        return Ok(response);
    }
}