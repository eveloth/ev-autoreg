using System.Security.Claims;
using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain;
using Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Policy = "UseRegistrar")]
[Route("api/[controller]")]
[ApiController]
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

    [HttpGet]
    public async Task<IActionResult> GetAllRules(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var rules = await _ruleService.GetAll(userId, pagination, cts);

        var response = new PagedResponse<RuleDto>(
            _mapper.Map<IEnumerable<RuleDto>>(rules),
            pagination
        );
        return Ok(response);
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetRule([FromRoute] int id, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var rule = await _ruleService.Get(id, userId, cts);

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(rule));
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> AddRule([FromBody] RuleRequest request, CancellationToken cts)
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

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

    [Route("{id:int}")]
    [HttpPut]
    public async Task<IActionResult> UpdateRule(
        [FromRoute] int id,
        [FromBody] RuleRequest request,
        CancellationToken cts
    )
    {
        await _validator.ValidateAndThrowAsync(request, cts);

        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

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

    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRule([FromRoute] int id, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

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