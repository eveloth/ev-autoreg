using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Domain;
using Api.Exceptions;
using Api.Services.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitofWork _unitofWork;

    public AuthenticationService(
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IUnitofWork unitofWork,
        IConfiguration config, ILogger<AuthenticationService> logger)
    {
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _unitofWork = unitofWork;
        _config = config;
        _logger = logger;
    }

    public async Task<Token> Register(string email, string password, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetByEmail(email, cts);

        if (existingUser is not null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1001]);
            throw e;
        }

        if (password == email)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1002]);
            throw e;
        }

        var passwordHash = _passwordHasher.HashPassword(password);
        var newUser = new UserModel { Email = email, PasswordHash = passwordHash };
        var createdUser = await _unitofWork.UserRepository.Create(newUser, cts);
        var result = await JoinUserRole(createdUser, cts);

        await _unitofWork.CommitAsync(cts);

        return await GenerateToken(result, cts);
    }

    public async Task<Token> Login(string email, string password, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetByEmail(email, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        if (existingUser.IsBlocked)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1003]);
            throw e;
        }

        if (
            _passwordHasher.VerifyPassword(existingUser.PasswordHash, password)
            != PasswordVerificationResult.Success
        )
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1005]);
            throw e;
        }

        var user = await JoinUserRole(existingUser, cts);
        return await GenerateToken(user, cts);
    }

    public async Task<Token> RefreshToken(RefreshToken token, CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    private async Task<Token> GenerateToken(User user, CancellationToken cts)
    {
        var keyString = _config["Jwt:Key"] ?? throw new NullConfigurationEntryException();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        var issuer = _config["Jwt:Issuer"] ?? throw new NullConfigurationEntryException();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.Role is not null)
        {
            var rolePermissionModels = await _unitofWork.RolePermissionRepository.Get(
                user.Role.Id,
                cts
            );

            await _unitofWork.CommitAsync(cts);

            var rolePermissions = _mapper.Map<RolePermission>(rolePermissionModels.ToList());

            if (rolePermissions.Permissions.Count != 0)
            {
                claims.AddRange(
                    rolePermissions.Permissions.Select(
                        permission => new Claim("Permission", permission.PermissionName)
                    )
                );
            }
        }

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
            Issuer = issuer,
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials
        };

        var token = handler.CreateToken(descriptor);

        _logger.LogInformation("User ID {UserId} was successfully logged in", user.Id);

        return new Token { JwtToken = handler.WriteToken(token) };
    }

    private async Task<User> JoinUserRole(UserModel userModel, CancellationToken cts)
    {
        if (userModel.RoleId is null)
        {
            return _mapper.Map<User>(userModel);
        }

        var roleModel = await _unitofWork.RoleRepository.Get(userModel.RoleId.Value, cts);

        var aggregationTable = new ValueTuple<UserModel, RoleModel>(userModel, roleModel!);
        return _mapper.Map<User>(aggregationTable);
    }
}