using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration config, IUserRolesRepository userRolesRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
        
    [Authorize(Roles = "manager, admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken cts)
    {
        return Ok(await _userRepository.GetAllUsers(cts));
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cts)
    {
        var user = await _userRepository.GetUserById(id, cts);

        return user is null ? NotFound(ErrorCode[2001]) : Ok(user);
    }

    [Route("{id:int}/email")]
    [HttpPatch]
    public async Task<IActionResult> UpdateEmail(int id, UserEmailDto email, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists) return NotFound("User not found");

        await _userRepository.UpdateUserEmail(id, email.NewEmail, cts);

        return Ok("Email was updated");
    }

    [Route("{id:int}/password")]
    [HttpPatch]
    public async Task<IActionResult> UpdatePassword(int id, UserPasswordDto password, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists) return NotFound("User not found");

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        await _userRepository.UpdateUserPassword(id, passwordHash, cts);

        return Ok("Password was updated");
    }

    [Authorize(Roles="Admin")]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(int id, UserPasswordDto password, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists) return NotFound("User not found");

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        await _userRepository.UpdateUserPassword(id, passwordHash, cts);
            
        return Ok("Password was reset");
    }

    [Route("{id:int}/block")]
    [HttpPost]
    public async Task<IActionResult> BlockUser(int id, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists) return NotFound("User not found.");
            
        await _userRepository.BlockUser(id, cts);
        return Ok("User was blocked.");
    }
        
    [Route("{id:int}/unblock")]
    [HttpPost]
    public async Task<IActionResult> UnblockUser(int id, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists) return NotFound("User not found.");
            
        await _userRepository.UnblockUser(id, cts);
        return Ok("User was unblocked.");
    }

    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cts)
    {
        var userExists = await _userRepository.DoesUserExist(id, cts);

        if (!userExists) return NotFound("User not found");
            
        await _userRepository.DeleteUser(id, cts);
        return Ok("User was deleted.");
    }
}