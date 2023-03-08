using EvAutoreg.Api.Contracts.Queries;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using EvAutoreg.Extensions;
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

    public async Task<IEnumerable<RolePermission>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var rolePermissions = await _unitofWork.RolePermissionRepository.GetAll(cts, filter);
        await _unitofWork.CommitAsync(cts);

        var listsOfRolePermissions = rolePermissions.GroupByIntoList(
            x => new { x.RoleId, x.RoleName }
        );

        var result = _mapper.Map<IEnumerable<RolePermission>>(listsOfRolePermissions);
        return result;
    }

    public async Task<RolePermission> Get(int id, CancellationToken cts)
    {
        var role = await _unitofWork.RoleRepository.Get(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (role is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var rolePermissions = await _unitofWork.RolePermissionRepository.Get(id, cts);
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(rolePermissions);
        return result;
    }

    public async Task<RolePermission> Add(Role role, CancellationToken cts)
    {
        var isRoleNameTaken = await _unitofWork.RoleRepository.DoesExist(role.RoleName, cts);

        if (isRoleNameTaken)
        {
            throw new ApiException().WithApiError(ErrorCode[2001]);
        }

        var roleModel = _mapper.Map<RoleModel>(role);
        var createdRole = await _unitofWork.RoleRepository.Add(roleModel, cts);
        var rolePermissions = await _unitofWork.RolePermissionRepository.Get(createdRole.Id, cts);
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(rolePermissions);
        return result;
    }

    public async Task<RolePermission> Rename(Role role, CancellationToken cts)
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
        var rolePermissions = await _unitofWork.RolePermissionRepository.Get(updatedRole.Id, cts);
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(rolePermissions);
        return result;
    }

    public async Task<RolePermission> Delete(int id, CancellationToken cts)
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

        var rolePermissions = await _unitofWork.RolePermissionRepository.Get(id, cts);
        await _unitofWork.RoleRepository.Delete(id, cts);
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(rolePermissions);
        return result;
    }

    public async Task<int> Count(CancellationToken cts)
    {
        var result = await _unitofWork.RoleRepository.Count(cts);
        await _unitofWork.CommitAsync(cts);
        return result;
    }
}