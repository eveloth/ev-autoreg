using Api.Contracts;
using Api.Domain;
using Api.Exceptions;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using Extensions;
using MapsterMapper;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

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

        var rolePermissions = await _unitofWork.RolePermissionRepository.GetAll(filter, cts);
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

        var doesPermissionExist = await _unitofWork.PermissionRepository.DoesExist(
            rolePermissionModel.PermissionId!.Value,
            cts
        );

        if (!doesPermissionExist)
        {
            throw new ApiException().WithApiError(ErrorCode[3004]);
        }

        var doesCorrelationExist =
            await _unitofWork.RolePermissionRepository.DoesCorrelationExist(rolePermissionModel, cts);

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

        var doesCorrelationExist =
            await _unitofWork.RolePermissionRepository.DoesCorrelationExist(rolePermissionModel, cts);

        if (!doesCorrelationExist)
        {
            throw new ApiException().WithApiError(ErrorCode[4004]);
        }

        var deletedCorrelation = await _unitofWork.RolePermissionRepository.RemovePermissionFromRole(
            rolePermissionModel,
            cts
        );
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<RolePermission>(deletedCorrelation);
        return result;
    }
}