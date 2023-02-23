using EvAutoreg.Api.Contracts;
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

public class RolePermissionService : IRolePermissionService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public RolePermissionService(IMapper mapper, IUnitofWork unitofWork)
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

    public async Task<RolePermission> AddPermissionToRole(
        RolePermission rolePermission,
        CancellationToken cts
    )
    {
        var rolePermissionModel = _mapper.Map<RolePermissionModel>(rolePermission);

        var role = await _unitofWork.RoleRepository.Get(rolePermissionModel.RoleId, cts);

        if (role is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var permission = await _unitofWork.PermissionRepository.Get(
            rolePermissionModel.PermissionId!.Value,
            cts
        );

        if (permission is null)
        {
            throw new ApiException().WithApiError(ErrorCode[3004]);
        }

        if (permission.IsPrivelegedPermission)
        {
            throw new ApiException().WithApiError(ErrorCode[3002]);
        }

        var doesCorrelationExist = await _unitofWork.RolePermissionRepository.DoesCorrelationExist(
            rolePermissionModel,
            cts
        );

        if (doesCorrelationExist)
        {
            throw new ApiException().WithApiError(ErrorCode[4001]);
        }

        var createdCorrelation = await _unitofWork.RolePermissionRepository.AddPermissionToRole(
            rolePermissionModel,
            cts
        );
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(createdCorrelation);
        return result;
    }

    public async Task<RolePermission> AddPrivelegedPermissionToRole(
        RolePermission rolePermission,
        CancellationToken cts
    )
    {
        var rolePermissionModel = _mapper.Map<RolePermissionModel>(rolePermission);

        var role = await _unitofWork.RoleRepository.Get(rolePermissionModel.RoleId, cts);

        if (role is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var permission = await _unitofWork.PermissionRepository.Get(
            rolePermissionModel.PermissionId!.Value,
            cts
        );

        if (permission is null)
        {
            throw new ApiException().WithApiError(ErrorCode[3004]);
        }

        var doesCorrelationExist = await _unitofWork.RolePermissionRepository.DoesCorrelationExist(
            rolePermissionModel,
            cts
        );

        if (doesCorrelationExist)
        {
            throw new ApiException().WithApiError(ErrorCode[4001]);
        }

        var createdCorrelation = await _unitofWork.RolePermissionRepository.AddPermissionToRole(
            rolePermissionModel,
            cts
        );

        role.IsPrivelegedRole = true;
        await _unitofWork.RoleRepository.ChangePriveleges(role, cts);

        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(createdCorrelation);
        result.Role.IsPrivelegedRole = true;
        return result;
    }

    public async Task<RolePermission> RemovePermissionFromRole(
        RolePermission rolePermission,
        CancellationToken cts
    )
    {
        var rolePermissionModel = _mapper.Map<RolePermissionModel>(rolePermission);

        var role = await _unitofWork.RoleRepository.Get(rolePermissionModel.RoleId, cts);

        if (role is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var doesCorrelationExist = await _unitofWork.RolePermissionRepository.DoesCorrelationExist(
            rolePermissionModel,
            cts
        );

        if (!doesCorrelationExist)
        {
            throw new ApiException().WithApiError(ErrorCode[4004]);
        }
        var permission = await _unitofWork.PermissionRepository.Get(
            rolePermissionModel.PermissionId!.Value,
            cts
        );

        if (permission!.IsPrivelegedPermission)
        {
            throw new ApiException().WithApiError(ErrorCode[3002]);
        }

        var deletedCorrelation =
            await _unitofWork.RolePermissionRepository.RemovePermissionFromRole(
                rolePermissionModel,
                cts
            );
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(deletedCorrelation);
        return result;
    }

    public async Task<RolePermission> RemovePrivelegedPermissionFromRole(
        RolePermission rolePermission,
        CancellationToken cts
    )
    {
        var rolePermissionModel = _mapper.Map<RolePermissionModel>(rolePermission);

        var role = await _unitofWork.RoleRepository.Get(rolePermissionModel.RoleId, cts);

        if (role is null)
        {
            throw new ApiException().WithApiError(ErrorCode[2004]);
        }

        var doesCorrelationExist = await _unitofWork.RolePermissionRepository.DoesCorrelationExist(
            rolePermissionModel,
            cts
        );

        if (!doesCorrelationExist)
        {
            throw new ApiException().WithApiError(ErrorCode[4004]);
        }

        var deletedCorrelation =
            await _unitofWork.RolePermissionRepository.RemovePermissionFromRole(
                rolePermissionModel,
                cts
            );
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(deletedCorrelation);

        var rolePermissionsModelsToCheck = await _unitofWork.RolePermissionRepository.Get(
            rolePermissionModel.RoleId,
            cts
        );
        var rolePermissionsToCheck = _mapper.Map<RolePermission>(rolePermissionsModelsToCheck);

        if (rolePermissionsToCheck.Permissions.Any(x => x.IsPrivelegedPermission))
        {
            return result;
        }

        role.IsPrivelegedRole = false;
        await _unitofWork.RoleRepository.ChangePriveleges(role, cts);
        await _unitofWork.CommitAsync(cts);

        result.Role.IsPrivelegedRole = false;
        return result;
    }

    public async Task<int> Count(CancellationToken cts)
    {
        var result = await _unitofWork.RolePermissionRepository.Count(cts);
        await _unitofWork.CommitAsync(cts);
        return result;
    }
}