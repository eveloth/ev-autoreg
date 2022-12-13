using Api.Contracts;
using Api.Domain;
using Api.Exceptions;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;
    private readonly IPasswordHasher _hasher;

    public UserService(IMapper mapper, IUnitofWork unitofWork, IPasswordHasher hasher)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
        _hasher = hasher;
    }

    public async Task<IEnumerable<User>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var users = await _unitofWork.UserRepository.GetAll(filter, cts);
        await _unitofWork.CommitAsync(cts);

        var result = users.Select(x => JoinUserRole(x, cts).Result);
        return result;
    }

    public async Task<User> Get(int id, CancellationToken cts)
    {
        var user = await _unitofWork.UserRepository.GetById(id, cts);

        if (user is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[1004]);
            throw e;
        }

        var result = await JoinUserRole(user, cts);
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

        var result = await JoinUserRole(user, cts);
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

        var result = await JoinUserRole(updatedUser, cts);
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
        await _unitofWork.CommitAsync(cts);

        var result = await JoinUserRole(updatedUser, cts);
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

        var userModel = _mapper.Map<UserModel>(user);
        var updatedUser = await _unitofWork.UserRepository.AddUserToRole(userModel, cts);

        var result = await JoinUserRole(updatedUser, cts);
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

        var updatedUser = await _unitofWork.UserRepository.RemoveUserFromRole(id, cts);

        var result = await JoinUserRole(updatedUser, cts);
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

        var updatedUser = await _unitofWork.UserRepository.Block(id, cts);

        var result = await JoinUserRole(updatedUser, cts);
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

        var updatedUser = await _unitofWork.UserRepository.Unblock(id, cts);

        var result = await JoinUserRole(updatedUser, cts);
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

        var result = await JoinUserRole(updatedUser, cts);
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

        var updatedUser = await _unitofWork.UserRepository.Restore(id, cts);

        var result = await JoinUserRole(updatedUser, cts);
        return result;
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