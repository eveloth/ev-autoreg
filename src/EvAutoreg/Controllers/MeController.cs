using System.Security.Claims;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MeController : ControllerBase
{
    private readonly ILogger<MeController> _logger;
    private readonly IUnitofWork _unitofWork;
    private readonly IAuthenticationService _authService;
    private readonly IPasswordHasher _passwordHasher;

    public MeController(
        ILogger<MeController> logger,
        IUnitofWork unitofWork,
        IAuthenticationService authService,
        IPasswordHasher passwordHasher
    )
    {
        _logger = logger;
        _unitofWork = unitofWork;
        _authService = authService;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var me = await _unitofWork.UserRepository.GetUserProfle(userId, cts);

        await _unitofWork.CommitAsync(cts);

        return Ok(me);
    }

    [Route("email")]
    [HttpPost]
    public async Task<IActionResult> UpdateEmail(UserEmailDto email, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var newEmail = email.NewEmail.ToLower();

        if (!_authService.IsEmailValid(newEmail))
        {
            return BadRequest(ErrorCode[1001]);
        }

        try
        {
            var userProfile = await _unitofWork.UserRepository.UpdateUserEmail(
                userId,
                newEmail,
                cts
            );

            await _unitofWork.CommitAsync(cts);

            _logger.LogInformation(
                "Email was updated to {NewEmail} for user ID {UserId}",
                userProfile.Email,
                userProfile.Id
            );
            return Ok(userProfile);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [Route("password")]
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(UserPasswordDto password, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var user = await _unitofWork.UserRepository.GetUserById(userId, cts);
        var email = user!.Email;

        if (!_authService.IsPasswordValid(email, password.NewPassword))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var passwordHash = _passwordHasher.HashPassword(password.NewPassword);

        try
        {
            var userWithChangedPassword = await _unitofWork.UserRepository.UpdateUserPassword(
                userId,
                passwordHash,
                cts
            );

            await _unitofWork.CommitAsync(cts);

            var id = new { UserId = userWithChangedPassword };

            _logger.LogInformation("Password was changed for user ID {UserId}", id.UserId);
            return Ok(id);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateUserProfile(
        UserProfileDto profile,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        try
        {
            var user = await _unitofWork.UserRepository.UpdateUserProfile(
                userId,
                profile.FisrtName,
                profile.LastName,
                cts
            );

            await _unitofWork.CommitAsync(cts);

            _logger.LogInformation("User profile was updated for user ID {UserId}", userId);
            return Ok(user);
        }
        catch (NpgsqlException e)
        {
            _logger.LogError("{ErrorMessage}", e.Message);
            return StatusCode(500, ErrorCode[9001]);
        }
    }
}
