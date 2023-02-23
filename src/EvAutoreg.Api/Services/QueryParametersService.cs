using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Mapping;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using EvAutoreg.Extensions;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class QueryParametersService : IQueryParametersService
{
    private readonly IMapper _mapper;
    private readonly IMappingHelper _mappingHelper;
    private readonly IUnitofWork _unitofWork;

    public QueryParametersService(
        IMapper mapper,
        IUnitofWork unitofWork,
        IMappingHelper mappingHelper
    )
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
        _mappingHelper = mappingHelper;
    }

    public async Task<IEnumerable<QueryParameters>> GetAll(
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var queryParameters = await _unitofWork.QueryParametersRepository.GetAll(cts, filter);
        await _unitofWork.CommitAsync(cts);

        var result = queryParameters.Select(x => _mappingHelper.JoinIssueType(x, cts).Result);
        return result;
    }

    public async Task<IEnumerable<QueryParameters>> Get(int issueTypeId, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(issueTypeId, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var queryParameters = await _unitofWork.QueryParametersRepository.Get(issueTypeId, cts);
        await _unitofWork.CommitAsync(cts);

        var result = queryParameters.Select(x => _mappingHelper.JoinIssueType(x, cts).Result);
        return result;
    }

    public async Task<QueryParameters> Add(QueryParameters parameters, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(parameters.IssueType.Id, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var queryParameters = (
            await _unitofWork.QueryParametersRepository.Get(parameters.IssueType.Id, cts)
        ).ToList();

        if (queryParameters.Any(x => x.Id == parameters.Id))
        {
            throw new ApiException().WithApiError(ErrorCode[10001]);
        }

        if (queryParameters.Any(x => x.ExecutionOrder == parameters.ExecutionOrder))
        {
            throw new ApiException().WithApiError(ErrorCode[10002]);
        }

        var queryParametersModel = _mapper.Map<QueryParametersModel>(parameters);
        var createdQueryParameters = await _unitofWork.QueryParametersRepository.Add(
            queryParametersModel,
            cts
        );
        await _unitofWork.CommitAsync(cts);
        var result = await _mappingHelper.JoinIssueType(createdQueryParameters, cts);
        return result;
    }

    public async Task<QueryParameters> Update(QueryParameters parameters, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(parameters.IssueType.Id, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var queryParameters = (
            await _unitofWork.QueryParametersRepository.Get(parameters.IssueType.Id, cts)
        ).ToList();

        if (queryParameters.No(x => x.Id == parameters.Id))
        {
            throw new ApiException().WithApiError(ErrorCode[10004]);
        }

        if (queryParameters.Any(x => x.ExecutionOrder == parameters.ExecutionOrder))
        {
            throw new ApiException().WithApiError(ErrorCode[10002]);
        }

        var queryParametersModel = _mapper.Map<QueryParametersModel>(parameters);
        var updatedQueryParameters = await _unitofWork.QueryParametersRepository.Update(
            queryParametersModel,
            cts
        );
        await _unitofWork.CommitAsync(cts);
        var result = await _mappingHelper.JoinIssueType(updatedQueryParameters, cts);
        return result;
    }

    public async Task<QueryParameters> Delete(int id, int issueTypeId, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(issueTypeId, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var queryParameters = await _unitofWork.QueryParametersRepository.Get(issueTypeId, cts);

        if (queryParameters.No(x => x.Id == id))
        {
            throw new ApiException().WithApiError(ErrorCode[10004]);
        }

        var deletedQueryParameters = await _unitofWork.QueryParametersRepository.Delete(
            id,
            issueTypeId,
            cts
        );
        await _unitofWork.CommitAsync(cts);
        var result = await _mappingHelper.JoinIssueType(deletedQueryParameters, cts);
        return result;
    }

    public async Task<int> Count(CancellationToken cts)
    {
        var result = await _unitofWork.QueryParametersRepository.Count(cts);
        await _unitofWork.CommitAsync(cts);
        return result;
    }
}