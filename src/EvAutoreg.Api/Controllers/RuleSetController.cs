using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EvAutoreg.Api.Controllers;

[Authorize(Policy = "UseRegistrar")]
[Route("api/[controller]")]
[ApiController]
[Tags("Rule set and rule management")]
public class RuleSetController : ControllerBase
{
    private readonly ILogger<RuleSetController> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<RuleRequest> _ruleValidator;
    private readonly IRuleSetService _ruleSetService;

    public RuleSetController(
        ILogger<RuleSetController> logger,
        IMapper mapper,
        IValidator<RuleRequest> ruleValidator,
        IRuleSetService ruleSetService
    )
    {
        _logger = logger;
        _mapper = mapper;
        _ruleValidator = ruleValidator;
        _ruleSetService = ruleSetService;
    }

    /// <summary>
    /// Returns all rule sets created by the current user for the specified issue type
    /// </summary>
    /// <response code="200">Returns all rule sets created by the current user for the specified issue type</response>
    /// <response code="404">If an issue type doesn't exist</response>
    [HttpGet]
    [ProducesResponseType(typeof(Response<IEnumerable<RuleSetDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllRuleSetsForIssueType(
        [FromQuery] [BindRequired] int issueTypeId,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        var ruleSets = await _ruleSetService.GetAllForIssueType(userId, issueTypeId, cts);
        var response = new Response<IEnumerable<RuleSetDto>>(
            _mapper.Map<IEnumerable<RuleSetDto>>(ruleSets)
        );

        return Ok(response);
    }

    /// <summary>
    /// Creates a rule set for the specified issue type
    /// </summary>
    /// <response code="200">Creates a rule set for the specified issue type</response>
    /// <response code="404">If an issue type doesn't exist</response>
    [HttpPost]
    [ProducesResponseType(typeof(Response<IEnumerable<RuleSetDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> CreateRuleSetForIssueType(
        [FromBody] RuleSetRequest request,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        var ruleSet = _mapper.Map<RuleSet>(request);
        ruleSet.OwnerUserId = userId;

        var updatedRuleSets = await _ruleSetService.Add(ruleSet, cts);
        var response = new Response<IEnumerable<RuleSetDto>>(
            _mapper.Map<IEnumerable<RuleSetDto>>(updatedRuleSets)
        );

        _logger.LogInformation(
            "A new rule set for issue type ID {IssueTypeId} was created by user ID {UserId}",
            request.IssueTypeId,
            userId
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes specified rule set
    /// </summary>
    /// <response code="200">Deletes specified rule set</response>
    /// <response code="404">If a rule set doesn't exist</response>
    [Route("{id:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<IEnumerable<RuleSetDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteRuleSet([FromRoute] int id, CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

        var updatedRuleSets = await _ruleSetService.Delete(userId, id, cts);
        var response = new Response<IEnumerable<RuleSetDto>>(
            _mapper.Map<IEnumerable<RuleSetDto>>(updatedRuleSets)
        );

        _logger.LogInformation(
            "Rule set ID {RuleSetId} was deleted by user ID {UserId}",
            id,
            userId
        );

        return Ok(response);
    }

    /// <summary>
    /// Adds a rule to the specified rule set
    /// </summary>
    /// <response code="200">Adds a rule to the specified rule set</response>
    /// <response code="404">If a rule set doesn't exist</response>
    [Route("{id:int}/rule")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<RuleSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AddRuleToARuleSet(
        [FromRoute] int id,
        [FromBody] RuleRequest request,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        await _ruleValidator.ValidateAndThrowAsync(request, cts);

        var rule = _mapper.Map<Rule>(request);
        rule.RuleSetId = id;
        var updatedRuleSet = await _ruleSetService.AddEntry(userId, rule, cts);

        var response = new Response<RuleSetDto>(_mapper.Map<RuleSetDto>(updatedRuleSet));

        _logger.LogInformation(
            "A new rule was added to the rule set ID {RuleSetid} by user ID {UserId}",
            id,
            userId
        );

        return Ok(response);
    }

    /// <summary>
    /// Updates a rule in the specified rule set
    /// </summary>
    /// <response code="200">Updates a rule in the specified rule set</response>
    /// <response code="404">If a rule set doesn't exist</response>
    [Route("{ruleSetId:int}/rule/{ruleId:int}")]
    [HttpPut]
    [ProducesResponseType(typeof(Response<RuleSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateRuleInARuleSet(
        [FromRoute] int ruleSetId,
        [FromRoute] int ruleId,
        [FromBody] RuleRequest request,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        await _ruleValidator.ValidateAndThrowAsync(request, cts);

        var rule = _mapper.Map<Rule>(request);
        rule.RuleSetId = ruleSetId;
        rule.Id = ruleId;
        var updatedRuleSet = await _ruleSetService.UpdateEntry(userId, rule, cts);

        var response = new Response<RuleSetDto>(_mapper.Map<RuleSetDto>(updatedRuleSet));

        _logger.LogInformation(
            "Rule ID {RuleId} was updated in the rule set ID {RuleSetid} by user ID {UserId}",
            ruleId,
            ruleSetId,
            userId
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes a rule from a rule set
    /// </summary>
    /// <response code="200">Deletes a rule from a rule set</response>
    /// <response code="404">If a rule set or a rule doesn't exist</response>
    [Route("{ruleSetId:int}/rule/{ruleId:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<RuleSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteRuleFromARuleSet(
        [FromRoute] int ruleSetId,
        [FromRoute] int ruleId,
        CancellationToken cts
    )
    {
        var userId = HttpContext.GetUserId();

        var updatedRuleSet = await _ruleSetService.DeleteEntry(userId, ruleSetId, ruleId, cts);

        var response = new Response<RuleSetDto>(_mapper.Map<RuleSetDto>(updatedRuleSet));

        _logger.LogInformation(
            "Rule ID {RuleId} was deleted from the rule set ID {RuleSetid} by user ID {UserId}",
            ruleId,
            ruleSetId,
            userId
        );

        return Ok(response);
    }
}