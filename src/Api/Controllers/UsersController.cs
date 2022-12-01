using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Api.Errors.ErrorCodes;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;
    private readonly IAuthenticationService _authService;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(
        ILogger<UsersController> logger,
        IAuthenticationService authService,
        IPasswordHasher passwordHasher,
        IUnitofWork unitofWork, 
        IMapper mapper)
    {
        _logger = logger;
        _authService = authService;
        _passwordHasher = passwordHasher;
        _unitofWork = unitofWork;
        _mapper = mapper;
    }

    [Authorize(Policy = "ReadUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);
        var users = await _unitofWork.UserRepository.GetAllUserProfiles(paginationFilter, cts);

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<UserProfileDto>(
            _mapper.Map<IEnumerable<UserProfileDto>>(users),
            pagination
        );

        return Ok(response);
    }

    [Authorize]
    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cts)
    {
        var user = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        await _unitofWork.CommitAsync(cts);

        return user is null
            ? NotFound(ErrorCode[2001])
            : Ok(new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(user)));
    }

    [Authorize(Policy = "ResetUserPasswords")]
    [Route("{id:int}/password/reset")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] int id,
        [FromBody] UserPasswordRequest request,
        CancellationToken cts
    )
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        if (existingUser is null)
        {
            return NotFound(ErrorCode[2001]);
        }

        if (!_authService.IsPasswordValid(existingUser.Email, request.NewPassword))
        {
            return BadRequest(ErrorCode[1002]);
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);

        var userWithPassswordReset = await _unitofWork.UserRepository.UpdateUserPassword(
            id,
            passwordHash,
            cts
        );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("Password was reset for user ID {UserId}", userWithPassswordReset);

        var response = new Response<int>(userWithPassswordReset);

        return Ok(response);
    }

    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/block")]
    [HttpPost]
    public async Task<IActionResult> BlockUser([FromRoute] int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        if (existingUser is null || existingUser.IsDeleted)
        {
            return NotFound(ErrorCode[2001]);
        }

        var blockedUser = await _unitofWork.UserRepository.BlockUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was blocked", blockedUser.Id);
        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(blockedUser));

        return Ok(response);
    }

    [Authorize(Policy = "BlockUsers")]
    [Route("{id:int}/unblock")]
    [HttpPost]
    public async Task<IActionResult> UnblockUser([FromRoute] int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetUserProfle(id, cts);

        if (existingUser is null || existingUser.IsDeleted)
        {
            return NotFound(ErrorCode[2001]);
        }

        var unblockedUser = await _unitofWork.UserRepository.UnblockUser(id, cts);

        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation("User ID {UserId} was unblocked", unblockedUser.Id);
        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(unblockedUser));

        return Ok(response);
    }

    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromRoute] int id, CancellationToken cts)
    {
        var userExists = await _unitofWork.UserRepository.DoesUserExist(id, cts);

        if (!userExists)
        {
            return NotFound("User not found");
        }

        var deletedUser = await _unitofWork.UserRepository.DeleteUser(id, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("User ID {UserId} was deleted", deletedUser.Id);

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(deletedUser));

        return Ok(response);
    }

    [Authorize(Policy = "DeleteUsers")]
    [Route("{id:int}/restore")]
    [HttpPost]
    public async Task<IActionResult> RestoreUser([FromRoute] int id, CancellationToken cts)
    {
        var userExists = await _unitofWork.UserRepository.DoesUserExist(id, cts);

        if (!userExists)
        {
            return NotFound("User not found");
        }

        var restoredUser = await _unitofWork.UserRepository.RestoreUser(id, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("User ID {UserId} was restored", restoredUser.Id);

        var response = new Response<UserProfileDto>(_mapper.Map<UserProfileDto>(restoredUser));

        return Ok(response);
    }
}