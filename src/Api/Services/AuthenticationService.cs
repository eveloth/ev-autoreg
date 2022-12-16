using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Domain;
using Api.Errors;
using Api.Exceptions;
using Api.Mapping;
using Api.Options;
using Api.Redis;
using Api.Redis.Entities;
using Api.Services.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using Grpc.Core;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IMapper _mapper;
    private readonly IMappingHelper _mappingHelper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitofWork _unitofWork;
    private readonly JwtOptions _jwtOptions;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ITokenDb _tokenDb;

    public AuthenticationService(
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IUnitofWork unitofWork,
        ILogger<AuthenticationService> logger,
        IMappingHelper mappingHelper,
        JwtOptions jwtOptions,
        TokenValidationParameters tokenValidationParameters,
        ITokenDb tokenDb
    )
    {
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _unitofWork = unitofWork;
        _logger = logger;
        _mappingHelper = mappingHelper;
        _jwtOptions = jwtOptions;
        _tokenValidationParameters = tokenValidationParameters;
        _tokenDb = tokenDb;
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
        var result = await _mappingHelper.JoinUserRole(createdUser, cts);

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

        var user = await _mappingHelper.JoinUserRole(existingUser, cts);
        return await GenerateToken(user, cts);
    }

    public async Task<Token> RefreshToken(Token token, CancellationToken cts)
    {
        var validatedToken = GetPrincipalFromToken(token.JwtToken);

        if (validatedToken is null)
        {
            Thrower.ThrowApiException(ErrorCode[1006]);
        }

        var expiryDateUnix = long.Parse(
            validatedToken!.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value
        );
        var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
            expiryDateUnix
        );

        if (expiryDateUtc > DateTime.UtcNow)
        {
            Thrower.ThrowApiException(ErrorCode[1008]);
        }

        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        var storedToken = await _tokenDb.GetRefreshToken(token.RefreshToken);

        if (storedToken is null)
        {
            Thrower.ThrowApiException(ErrorCode[1009]);
        }

        if (DateTime.UtcNow > storedToken!.TokenInfo!.ExpiryDate)
        {
            Thrower.ThrowApiException(ErrorCode[1006]);
        }

        if (storedToken.TokenInfo.Invalidated)
        {
            Thrower.ThrowApiException(ErrorCode[1010]);
        }

        if (storedToken.TokenInfo.Used)
        {
            Thrower.ThrowApiException(ErrorCode[1011]);
        }

        if (storedToken.TokenInfo.Jti != jti)
        {
            Thrower.ThrowApiException(ErrorCode[1012]);
        }

        storedToken.TokenInfo.Used = true;
        await _tokenDb.SaveRefreshToken(storedToken);

        var userId = int.Parse(
            validatedToken.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value
        );
        var userModel = await _unitofWork.UserRepository.GetById(userId, cts);
        var user = _mapper.Map<User>(userModel!);

        return await GenerateToken(user, cts);
    }

    private async Task<Token> GenerateToken(User user, CancellationToken cts)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var issuer = _jwtOptions.Issuer;
        var lifetime = _jwtOptions.Lifetime;
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
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

        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
            Issuer = issuer,
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials = credentials
        };

        var token = handler.CreateToken(descriptor);

        var resfreshTokenInfo = new TokenInfo
        {
            UserId = user.Id,
            Jti = token.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
            Invalidated = false,
            Used = false
        };
        var refreshToken = new RefreshToken(resfreshTokenInfo);

        await _tokenDb.SaveRefreshToken(refreshToken);

        _logger.LogInformation("User ID {UserId} was successfully logged in", user.Id);

        return new Token
        {
            JwtToken = handler.WriteToken(token),
            RefreshToken = refreshToken.Token
        };
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(
                jwtToken,
                _tokenValidationParameters,
                out var validatedToken
            );

            return !IsValidSecurityAlgorythm(validatedToken) ? null : principal;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private bool IsValidSecurityAlgorythm(SecurityToken token)
    {
        return token is JwtSecurityToken securityToken
            && securityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            );
    }
}