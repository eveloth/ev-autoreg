using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IUserRepository _userRepository;

    public RoleController(IUserRolesRepository userRolesRepository, IUserRepository userRepository)
    {
        _userRolesRepository = userRolesRepository;
        _userRepository = userRepository;
    }

    [Authorize(Roles="admin")]
    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cts)
    {
        var roles = await _userRolesRepository.GetRoles(cts);

        return Ok(roles);
    }

    [Authorize(Roles="admin")]
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleDto role, CancellationToken cts)
    {
        var roleName = role.RoleName.ToLower();
            
        await _userRolesRepository.AddRole(roleName, cts);

        return Ok($"{roleName} role was added.");
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(int id, RoleDto role, CancellationToken cts)
    {
        var newRoleName = role.RoleName.ToLower();

        await _userRolesRepository.ChangeRoleName(id, newRoleName, cts);

        return Ok("Rolename was changed");
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cts)
    {
        await _userRolesRepository.DeleteRole(id, cts);

        return Ok("Role was deleted");
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpGet]
    public async Task<IActionResult> GetAllUserRoles(CancellationToken cts)
    {
        var userRoles = await _userRolesRepository.GetAllUserRoles(cts);

        return Ok(userRoles);
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserRole(int id, CancellationToken cts)
    {
        var userRole = await _userRolesRepository.GetUserRole(id, cts);

        return Ok(userRole);
    } 

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(UserRoleDto userRole, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(userRole.UserId, cts);

        if (!userExists)
        {
            return NotFound("User doesn't exist");
        }

        var roleExists = await _userRolesRepository.DoesRoleExist(userRole.RoleId, cts);

        if (!roleExists)
        {
            return NotFound("Role doesn't exist.");
        }

        var isOperationSuccessfull = await _userRolesRepository.SetUserRole(userRole.UserId, userRole.RoleId, cts);

        if (!isOperationSuccessfull)
        {
            return BadRequest("Something went wrong.");
        }

        return Ok("Successfully added user to role");
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUserFromRole(UserRoleDto userRole, CancellationToken cts)
    {
        var recordExists = await _userRolesRepository.DoesRecordExist(userRole.UserId, userRole.RoleId, cts);

        if (!recordExists) return NotFound("No user with the specified role found.");

        var isOperationSuccessfull = await _userRolesRepository.DeleteUserFromRole(userRole.UserId, userRole.RoleId, cts);

        if (!isOperationSuccessfull) return BadRequest("Something went wrong");

        return Ok("Successfuly removed user from role.");
    }
}