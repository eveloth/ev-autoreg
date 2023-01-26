using Api.Contracts;
using Api.Domain;
using Api.Exceptions;
using Api.Mapping;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

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

        var queryParameters = await _unitofWork.QueryParametersRepository.GetAll(filter, cts);
        await _unitofWork.CommitAsync(cts);

        var result = queryParameters.Select(x => _mappingHelper.JoinIssueType(x, cts).Result);
        return result;
    }

    public async Task<QueryParameters> Get(int issueTypeId, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(issueTypeId, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var queryParameters = await _unitofWork.QueryParametersRepository.Get(issueTypeId, cts);
        await _unitofWork.CommitAsync(cts);

        if (queryParameters is null)
        {
            throw new ApiException().WithApiError(ErrorCode[10004]);
        }

        var result = await _mappingHelper.JoinIssueType(queryParameters!, cts);
        return result;
    }

    public async Task<QueryParameters> Upsert(QueryParameters parameters, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(parameters.IssueType.Id, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var queryParametersModel = _mapper.Map<QueryParametersModel>(parameters);
        var upsertedQueryParameters = await _unitofWork.QueryParametersRepository.Upsert(queryParametersModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = await _mappingHelper.JoinIssueType(upsertedQueryParameters, cts);
        return result;
    }
}