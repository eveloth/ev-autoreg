using System.Security.Claims;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Requests;
using EvAutoreg.Contracts.Responses;
using EvAutoreg.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MeController : ControllerBase
{
    private readonly ILogger<MeController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;
    private readonly IAuthenticationService _authService;
    private readonly IPasswordHasher _passwordHasher;

    public MeController(
        ILogger<MeController> logger,
        IUnitofWork unitofWork,
        IAuthenticationService authService,
        IPasswordHasher passwordHasher,
        IMapper mapper
    )
    {
        _logger = logger;
        _unitofWork = unitofWork;
        _authService = authService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var me = await _unitofWork.UserRepository.GetUserProfle(userId, cts);

        await _unitofWork.CommitAsync(cts);

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(me!));

        return Ok(response);
    }

    [Route("email")]
    [HttpPost]
    public async Task<IActionResult> UpdateEmail(
        [FromBody] UserEmailRequest request,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        if (!_authService.IsEmailValid(request.NewEmail))
        {
            return BadRequest(ErrorCode[1001]);
        }

        var userProfile = await _unitofWork.UserRepository.UpdateUserEmail(userId, request.NewEmail, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Email was updated to {NewEmail} for user ID {UserId}",
            userProfile.Email,
            userProfile.Id
        );

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(userProfile));

        return Ok(response);
    }

    [Route("password")]
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(
        [FromBody] UserPasswordRequest request,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var user = await _unitofWork.UserRepository.GetUserById(userId, cts);
        var email = user!.Email;

        if (!_authService.IsPasswordValid(email, request.NewPassword))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);

        var userWithChangedPassword = await _unitofWork.UserRepository.UpdateUserPassword(
            userId,
            passwordHash,
            cts
        );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "Password was changed for user ID {UserId}",
            userWithChangedPassword
        );

        var response = new Response<int>(userWithChangedPassword);

        return Ok(response);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateUserProfile(
        [FromBody] UserProfileRequest request,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var user = await _unitofWork.UserRepository.UpdateUserProfile(
            userId,
            request.FisrtName,
            request.LastName,
            cts
        );

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User profile was updated for user ID {UserId}", userId);

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(user));

        return Ok(response);
    }
}