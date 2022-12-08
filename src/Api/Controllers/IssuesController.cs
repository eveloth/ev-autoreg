using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Responses;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Policy = "UseRegistrar")]
[Route("api/[controller]")]
[ApiController]
public class IssuesController : ControllerBase
{
    private readonly ILogger<IssuesController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public IssuesController(
        ILogger<IssuesController> logger,
        IMapper mapper,
        IUnitofWork unitofWork
    )
    {
        _logger = logger;
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllIssues(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var issues = await _unitofWork.IssueRepository.GetAll(paginationFilter, cts);

        var aggregationTable =
            new List<ValueTuple<IssueModel, UserProfileModel?, IssueTypeModel?>>();

        foreach (var issue in issues)
        {
            var registrar = await _unitofWork.UserRepository.GetUserProfle(
                issue.RegistrarId!.Value,
                cts
            );
            var issueType = await _unitofWork.IssueTypeRepository.Get(
                issue.IssueTypeId!.Value,
                cts
            );

            aggregationTable.Add(
                new ValueTuple<IssueModel, UserProfileModel?, IssueTypeModel?>
                {
                    Item1 = issue,
                    Item2 = registrar ?? null,
                    Item3 = issueType ?? null
                }
            );
        }

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<IssueDto>(
            _mapper.Map<IEnumerable<IssueDto>>(aggregationTable),
            pagination
        );

        return Ok(response);
    }
}