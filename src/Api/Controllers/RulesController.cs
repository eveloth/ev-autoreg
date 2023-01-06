using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Extensions;
using Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rule = Api.Domain.Rule;

namespace Api.Controllers;

[Authorize(Policy = "UseRegistrar")]
[Route("api/[controller]")]
[ApiController]
[Tags(("Rule management"))]
public class RulesController : ControllerBase
{
    private readonly ILogger<RulesController> _logger;
    private readonly IMapper _mapper;
    private readonly IRuleService _ruleService;
    private readonly IValidator<RuleRequest> _validator;

    public RulesController(
        ILogger<RulesController> logger,
        IMapper mapper,
        IRuleService ruleService,
        IValidator<RuleRequest> validator
    )
    {
        _logger = logger;
        _mapper = mapper;
        _ruleService = ruleService;
        _validator = validator;
    }

    /// <summary>
    /// Returns all issue analysis rules created by the current user
    /// </summary>
    /// <response code="200">Returns all issue analysis rules created by the current user</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RuleDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllRules(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        var rules = await _ruleService.GetAll(userId, pagination, cts);

        var response = new PagedResponse<RuleDto>(
            _mapper.Map<IEnumerable<RuleDto>>(rules),
            pagination
        );
        return Ok(response);
    }

    /// <summary>
    /// Returns the specified issue analysis rule created by the current user
    /// </summary>
    /// <response code="200">Returns the specified issue analysis rule created by the current user</response>
    /// <response code="404">If a rule doesn't exist</response>
    [Route("{id:int}")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<RuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetRule([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        var rule = await _ruleService.Get(id, userId, cts);

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(rule));
        return Ok(response);
    }

    /// <summary>
    /// Adds an issue analysis rule for the current user
    /// </summary>
    /// <response code="200">Adds an issue analysis rule for the current user</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="404">If a rule, or an issue type, or an issue field doesn't exist</response>
    [HttpPost]
    [ProducesResponseType(typeof(Response<RuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AddRule([FromBody] RuleRequest request, CancellationToken cts)
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var userId = HttpContext.GetUserId();

        var rule = _mapper.Map<Rule>(request);
        rule.OwnerUserId = userId;

        var createdRule = await _ruleService.Add(rule, cts);

        _logger.LogInformation(
            "Rule ID {RuleId} was added for user ID {UserId}",
            createdRule.Id,
            createdRule.OwnerUserId
        );

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(createdRule));
        return Ok(response);
    }

    /// <summary>
    /// Updates the specified issue analysis rule for the current user
    /// </summary>
    /// <response code="200">Updates the specified issue analysis rule for the current user</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="404">If a rule, or an issue type, or an issue field doesn't exist</response>
    [Route("{id:int}")]
    [HttpPut]
    [ProducesResponseType(typeof(Response<RuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateRule(
        [FromRoute] int id,
        [FromBody] RuleRequest request,
        CancellationToken cts
    )
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var userId = HttpContext.GetUserId();

        var rule = _mapper.Map<Rule>(request);
        rule.Id = id;
        rule.OwnerUserId = userId;

        var updatedRule = await _ruleService.Update(rule, cts);

        _logger.LogInformation(
            "Rule ID {RuleId} was updated for user ID {UserId}",
            updatedRule.Id,
            updatedRule.OwnerUserId
        );

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(updatedRule));
        return Ok(response);
    }

    /// <summary>
    /// Deletes the specified issue analysis rule for the current user
    /// </summary>
    /// <response code="200">Deletes the specified issue analysis rule for the current user</response>
    /// <response code="404">If a rule doesn't exist</response>
    [Route("{id:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<RuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteRule([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        var deletedRule = await _ruleService.Delete(id, userId, cts);

        _logger.LogInformation(
            "Rule ID {RuleId} was updated for user ID {UserId}",
            deletedRule.Id,
            deletedRule.OwnerUserId
        );

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(deletedRule));
        return Ok(response);
    }
}