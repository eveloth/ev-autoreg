using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Mapping;
using EvAutoreg.Api.Redis;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using Microsoft.VisualBasic.CompilerServices;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IMappingHelper _mappingHelper;
    private readonly IUnitofWork _unitofWork;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenDb _tokenDb;

    public UserService(
        IMapper mapper,
        IUnitofWork unitofWork,
        IPasswordHasher hasher,
        IMappingHelper mappingHelper,
        ITokenDb tokenDb
    )
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
        _hasher = hasher;
        _mappingHelper = mappingHelper;
        _tokenDb = tokenDb;
    }

    public async Task<IEnumerable<User>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var users = await _unitofWork.UserRepository.GetAll(filter, cts);
        await _unitofWork.CommitAsync(cts);

        var result = users.Select(x => _mappingHelper.JoinUserRole(x, cts).Result);
        return result;
    }

    public async Task<User> Get(int id, CancellationToken cts)
    {
        var user = await _unitofWork.UserRepository.GetById(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (user is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var result = await _mappingHelper.JoinUserRole(user, cts);
        return result;
    }

    public async Task<User> Get(string email, CancellationToken cts)
    {
        var user = await _unitofWork.UserRepository.GetByEmail(email, cts);
        await _unitofWork.CommitAsync(cts);

        if (user is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var result = await _mappingHelper.JoinUserRole(user, cts);
        return result;
    }

    public async Task<User> UpdateProfile(User user, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(user.Id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var userModel = _mapper.Map<UserModel>(user);
        var updatedUser = await _unitofWork.UserRepository.UpdateUserProfile(userModel, cts);
        await _unitofWork.CommitAsync(cts);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<User> ChangeEmail(User user, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(user.Id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var sameEmailUser = await _unitofWork.UserRepository.GetByEmail(user.Email, cts);

        if (sameEmailUser is not null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1001]);
            throw e;
        }

        var userModel = _mapper.Map<UserModel>(user);
        var updatedUser = await _unitofWork.UserRepository.UpdateUserProfile(userModel, cts);
        await _tokenDb.InvalidateRefreshToken(updatedUser.Id);
        await _unitofWork.CommitAsync(cts);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<int> ChangePassword(int id, string password, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        if (password == existingUser.Email)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1002]);
            throw e;
        }

        var passwordHash = _hasher.HashPassword(password);
        var userToUpdate = new UserModel { Id = id, PasswordHash = passwordHash };
        var updatedUserId = await _unitofWork.UserRepository.UpdatePassword(userToUpdate, cts);
        await _tokenDb.InvalidateRefreshToken(updatedUserId);
        await _unitofWork.CommitAsync(cts);

        return updatedUserId;
    }

    public async Task<User> AddUserToRole(User user, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(user.Id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var doesRoleExist = await _unitofWork.RoleRepository.DoesExist(user.Role!.Id, cts);

        if (!doesRoleExist)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[2004]);
            throw e;
        }

        if (existingUser.RoleId is not null && existingUser.RoleId == user.Role.Id)
        {
            return await _mappingHelper.JoinUserRole(existingUser, cts);
        }

        var userModel = _mapper.Map<UserModel>(user);
        var updatedUser = await _unitofWork.UserRepository.AddUserToRole(userModel, cts);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<User> RemoveUserFromRole(int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        if (existingUser.RoleId is null)
        {
            return _mapper.Map<User>(existingUser);
        }

        var updatedUser = await _unitofWork.UserRepository.RemoveUserFromRole(id, cts);
        await _unitofWork.CommitAsync(cts);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<User> Block(int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        if (existingUser.IsBlocked)
        {
            return await _mappingHelper.JoinUserRole(existingUser, cts);
        }

        var updatedUser = await _unitofWork.UserRepository.Block(id, cts);
        await _tokenDb.InvalidateRefreshToken(updatedUser.Id);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<User> Unblock(int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        if (!existingUser.IsBlocked)
        {
            return await _mappingHelper.JoinUserRole(existingUser, cts);
        }

        var updatedUser = await _unitofWork.UserRepository.Unblock(id, cts);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<User> Delete(int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(id, cts);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var updatedUser = await _unitofWork.UserRepository.Delete(id, cts);
        await _tokenDb.InvalidateRefreshToken(updatedUser.Id);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }

    public async Task<User> Restore(int id, CancellationToken cts)
    {
        var existingUser = await _unitofWork.UserRepository.GetById(id, cts, includeDeleted: true);

        if (existingUser is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        if (!existingUser.IsDeleted)
        {
            return await _mappingHelper.JoinUserRole(existingUser, cts);
        }

        var updatedUser = await _unitofWork.UserRepository.Restore(id, cts);

        var result = await _mappingHelper.JoinUserRole(updatedUser, cts);
        return result;
    }
}