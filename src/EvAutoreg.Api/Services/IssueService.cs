using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Mapping;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

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

        var issues = await _unitofWork.IssueRepository.GetAll(cts, filter);

        var result = issues.Select(x => _mappingHelper.JoinIssueTypeAndUser(x, cts).Result);
        return result;
    }

    public async Task<Issue> Get(int id, CancellationToken cts)
    {
        var existingIssue = await _unitofWork.IssueRepository.Get(id, cts);

        if (existingIssue is null)
        {
            throw new ApiException().WithApiError(ErrorCode[5004]);
        }

        var result = await _mappingHelper.JoinIssueTypeAndUser(existingIssue, cts);
        return result;
    }
}