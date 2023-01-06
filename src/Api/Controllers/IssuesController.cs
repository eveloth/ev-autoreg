using Api.Cache;
using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Policy = "UseRegistrar")]
[Route("api/[controller]")]
[ApiController]
[Tags("Issue management")]
public class IssuesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IIssueService _issueService;

    public IssuesController(IMapper mapper, IIssueService issueService)
    {
        _mapper = mapper;
        _issueService = issueService;
    }

    /// <summary>
    /// Returns all issues from the database
    /// </summary>
    /// <response code="200">Returns all issues from the database</response>
    [Cached(300)]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<IssueDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllIssues(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var issues = await _issueService.GetAll(pagination, cts);

        var response = new PagedResponse<IssueDto>(
            _mapper.Map<IEnumerable<IssueDto>>(issues),
            pagination
        );
        return Ok(response);
    }

    /// <summary>
    /// Returns the specified issue from the database
    /// </summary>
    /// <response code="200">Returns the specified issue from the database</response>
    /// <response code="404">If an issue doesn't exist</response>
    [Cached(300)]
    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(Response<IssueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetIssue([FromRoute] int id, CancellationToken cts)
    {
        var issue = await _issueService.Get(id, cts);

        var response = new Response<IssueDto>(_mapper.Map<IssueDto>(issue));
        return Ok(response);
    }
}