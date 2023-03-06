using EvAutoreg.Api.Contracts.Queries;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class IssueTypeService : IIssueTypeService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public IssueTypeService(IMapper mapper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<IssueType>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var issueTypes = await _unitofWork.IssueTypeRepository.GetAll(cts, filter);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IEnumerable<IssueType>>(issueTypes);
        return result;
    }

    public async Task<IssueType> Get(int id, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var result = _mapper.Map<IssueType>(issueType);
        return result;
    }

    public async Task<IssueType> Add(IssueType type, CancellationToken cts)
    {
        var issueTypeNameTaken = await _unitofWork.IssueTypeRepository.DoesExist(
            type.IssueTypeName,
            cts
        );

        if (issueTypeNameTaken)
        {
            throw new ApiException().WithApiError(ErrorCode[7001]);
        }

        var issueTypeModel = _mapper.Map<IssueTypeModel>(type);
        var createdIssueType = await _unitofWork.IssueTypeRepository.Add(issueTypeModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IssueType>(createdIssueType);
        return result;
    }

    public async Task<IssueType> Rename(IssueType type, CancellationToken cts)
    {
        var issueTypeNameTaken = await _unitofWork.IssueTypeRepository.DoesExist(
            type.IssueTypeName,
            cts
        );

        if (issueTypeNameTaken)
        {
            throw new ApiException().WithApiError(ErrorCode[7001]);
        }

        var issueTypeModel = _mapper.Map<IssueTypeModel>(type);
        var updatedIssueType = await _unitofWork.IssueTypeRepository.ChangeName(
            issueTypeModel,
            cts
        );
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IssueType>(updatedIssueType);
        return result;
    }

    public async Task<IssueType> Delete(int id, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(id, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var deletedIssueType = await _unitofWork.IssueTypeRepository.Delete(id, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IssueType>(deletedIssueType);
        return result;
    }

    public async Task<int> Count(CancellationToken cts)
    {
        var result = await _unitofWork.IssueTypeRepository.Count(cts);
        await _unitofWork.CommitAsync(cts);
        return result;
    }
}