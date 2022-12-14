using Api.Contracts;
using Api.Domain;
using Api.Exceptions;
using Api.Mapping;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

public class IssueService : IIssueService
{
    private readonly IMapper _mapper;
    private readonly IMappingHelper _mappingHelper;
    private readonly IUnitofWork _unitofWork;

    public IssueService(IMapper mapper, IMappingHelper mappingHelper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _mappingHelper = mappingHelper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<Issue>> GetAll(PaginationQuery paginationQuery, CancellationToken cts)
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var issues = await _unitofWork.IssueRepository.GetAll(filter, cts);

        var result = issues.Select(x => _mappingHelper.JoinIssueTypeAndUser(x, cts).Result);
        return result;
    }

    public async Task<Issue> Get(int id, CancellationToken cts)
    {
        var existingIssue = await _unitofWork.IssueRepository.Get(id, cts);

        if (existingIssue is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[5004]);
            throw e;
        }

        var result = await _mappingHelper.JoinIssueTypeAndUser(existingIssue, cts);
        return result;
    }
}