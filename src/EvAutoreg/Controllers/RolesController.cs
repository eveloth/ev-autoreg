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
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _userRolesRepository.GetRoles();

        return Ok(roles);
    }

    [Authorize(Roles="admin")]
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleDto role)
    {
        var roleName = role.RoleName.ToLower();
            
        await _userRolesRepository.AddRole(roleName);

        return Ok($"{roleName} role was added.");
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeRoleName(int id, RoleDto role)
    {
        var newRoleName = role.RoleName.ToLower();

        await _userRolesRepository.ChangeRoleName(id, newRoleName);

        return Ok("Rolename was changed");
    }

    [Authorize(Roles = "admin")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id)
    {
        await _userRolesRepository.DeleteRole(id);

        return Ok("Role was deleted");
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpGet]
    public async Task<IActionResult> GetAllUserRoles()
    {
        var userRoles = await _userRolesRepository.GetAllUserRoles();

        return Ok(userRoles);
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserRole(int id)
    {
        var userRole = await _userRolesRepository.GetUserRole(id);

        return Ok(userRole);
    } 

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpPost]
    public async Task<IActionResult> AddUserToRole(UserRoleDto userRole)
    {
        var userExists = await _userRepository.DoesUserExist(userRole.UserId);

        if (!userExists)
        {
            return NotFound("User doesn't exist");
        }

        var roleExists = await _userRolesRepository.DoesRoleExist(userRole.RoleId);

        if (!roleExists)
        {
            return NotFound("Role doesn't exist.");
        }

        var isOperationSuccessfull = await _userRolesRepository.SetUserRole(userRole.UserId, userRole.RoleId);

        if (!isOperationSuccessfull)
        {
            return BadRequest("Something went wrong.");
        }

        return Ok("Successfully added user to role");
    }

    [Authorize(Roles = "admin")]
    [Route("user-roles")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUserFromRole(UserRoleDto userRole)
    {
        var recordExists = await _userRolesRepository.DoesRecordExist(userRole.UserId, userRole.RoleId);

        if (!recordExists) return NotFound("No user with the specified role found.");

        var isOperationSuccessfull = await _userRolesRepository.DeleteUserFromRole(userRole.UserId, userRole.RoleId);

        if (!isOperationSuccessfull) return BadRequest("Something went wrong");

        return Ok("Successfuly removed user from role.");
    }
}