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
public class IssuesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IIssueService _issueService;

    public IssuesController(IMapper mapper, IIssueService issueService)
    {
        _mapper = mapper;
        _issueService = issueService;
    }

    [HttpGet]
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
}