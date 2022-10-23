using DataAccessLibrary.Models;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
        
    [HttpPost]
    public async Task<IActionResult> RegisterUser(UserCredentialsDto request)
    {
        var email = request.Email.ToLower();

        var userExists = await _userRepository.DoesUserExist(email);

        if (userExists) return BadRequest("User already exists.");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var newUser = new UserModel
        {
            Email = email,
            PasswordHash = passwordHash
        };

        await _userRepository.CreateUser(newUser);

        return Ok(newUser.Email);
    }

    [Route("/token")]
    [HttpPost]
    public async Task<IActionResult> Login(UserCredentialsDto request)
    {
        var email = request.Email.ToLower();

        var existingUser = await _userRepository.GetUserByEmail(email);

        if (existingUser is null)
        {
            return NotFound("User doesn't exist");
        }

        if (_passwordHasher.VerifyPassword(existingUser.PasswordHash, request.Password) == PasswordVerificationResult.Success)
        {
            return Ok("Loggen in.");
        }

        return BadRequest("Wrong credentials");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _userRepository.GetAllUsers());
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userRepository.GetUserById(id);

        return user is null ? NotFound("User not found.") : Ok(user);
    }

    [Route("{id:int}/email")]
    [HttpPatch]
    public async Task<IActionResult> UpdateEmail(int id, UserEmailDto email)
    {
        var userExists = await _userRepository.DoesUserExist(id);

        if (!userExists) return NotFound("User not found");

        await _userRepository.UpdateUserEmail(id, email.NewEmail);

        return Ok("Email was updated");
    }

    [Route("{id:int}/password")]
    [HttpPatch]
    public async Task<IActionResult> UpdatePassword(int id, UserPasswordDto password)
    {
        var userExists = await _userRepository.DoesUserExist(id);

        if (!userExists) return NotFound("User not found");

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        await _userRepository.UpdateUserPassword(id, passwordHash);

        return Ok("Password was updated");
    }

    //[Authorize(Roles="Admin)]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(int id, UserPasswordDto password)
    {
        var userExists = await _userRepository.DoesUserExist(id);

        if (!userExists) return NotFound("User not found");

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        await _userRepository.UpdateUserPassword(id, passwordHash);
            
        return Ok("Password was reset");
    }

    [Route("{id:int}/block")]
    [HttpPost]
    public async Task<IActionResult> BlockUser(int id)
    {
        var userExists = await _userRepository.DoesUserExist(id);

        if (!userExists) return NotFound("User not found.");
            
        await _userRepository.BlockUser(id);
        return Ok("User was blocked.");
    }
        
    [Route("{id:int}/unblock")]
    [HttpPost]
    public async Task<IActionResult> UnblockUser(int id)
    {
        var userExists = await _userRepository.DoesUserExist(id);

        if (!userExists) return NotFound("User not found.");
            
        await _userRepository.UnblockUser(id);
        return Ok("User was unblocked.");
    }

    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var userExists = await _userRepository.DoesUserExist(id);

        if (!userExists) return NotFound("User not found");
            
        await _userRepository.DeleteUser(id);
        return Ok("User was deleted.");
    }
}