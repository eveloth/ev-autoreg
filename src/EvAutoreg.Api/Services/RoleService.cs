using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class RoleService : IRoleService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public RoleService(IMapper mapper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<Role>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filer = _mapper.Map<PaginationFilter>(paginationQuery);

        var roles = await _unitofWork.RoleRepository.GetAll(filer, cts);
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<IEnumerable<Role>>(roles);
        return result;
    }

    public async Task<Role> Get(int id, CancellationToken cts)
    {
        var role = await _unitofWork.RoleRepository.Get(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (role is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var result = _mapper.Map<Role>(role);
        return result;
    }

    public async Task<Role> Add(Role role, CancellationToken cts)
    {
        var isRoleNameTaken = await _unitofWork.RoleRepository.DoesExist(role.RoleName, cts);

        if (isRoleNameTaken)
        {
            throw new ApiException().WithApiError(ErrorCode[2001]);
        }

        var roleModel = _mapper.Map<RoleModel>(role);
        var createdRole = await _unitofWork.RoleRepository.Add(roleModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<Role>(createdRole);
        return result;
    }

    public async Task<Role> Rename(Role role, CancellationToken cts)
    {
        var existingRole = await _unitofWork.RoleRepository.Get(role.Id, cts);

        if (existingRole is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var isRoleNameTaken = await _unitofWork.RoleRepository.DoesExist(role.RoleName, cts);

        if (isRoleNameTaken)
        {
            throw new ApiException().WithApiError(ErrorCode[2001]);
        }

        var roleModel = _mapper.Map<RoleModel>(role);
        var updatedRole = await _unitofWork.RoleRepository.ChangeName(roleModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<Role>(updatedRole);
        return result;
    }

    public async Task<Role> Delete(int id, CancellationToken cts)
    {
        var existingRole = await _unitofWork.RoleRepository.Get(id, cts);

        if (existingRole is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        if (existingRole.IsPrivelegedRole)
        {
            throw new ApiException().WithApiError(ErrorCode[2005]);
        }

        var deletedRole = await _unitofWork.RoleRepository.Delete(id, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<Role>(deletedRole);
        return result;
    }
}