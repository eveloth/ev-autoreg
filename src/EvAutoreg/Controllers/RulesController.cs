using System.Security.Claims;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Requests;
using EvAutoreg.Contracts.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Authorize(Policy = "UseRegistrar")]
[Route("api/[controller]")]
[ApiController]
public class RulesController : ControllerBase
{
    private readonly ILogger<RulesController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public RulesController(ILogger<RulesController> logger, IMapper mapper, IUnitofWork unitofWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitofWork = unitofWork;
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

        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var rules = await _unitofWork.RuleRepository.GetAllRules(userId, paginationFilter, cts);

        List<(RuleModel, IssueTypeModel?, IssueFieldModel?)> aggregationTable = new();

        foreach (var rule in rules)
        {
            var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
                rule.IssueTypeId,
                cts
            );
            var issueField = await _unitofWork.IssueFieldRepository.GetIssueField(
                rule.IssueFieldId,
                cts
            );

            aggregationTable.Add(
                new ValueTuple<RuleModel, IssueTypeModel?, IssueFieldModel?>
                {
                    Item1 = rule,
                    Item2 = issueType ?? null,
                    Item3 = issueField ?? null
                }
            );
        }

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<RuleDto>(
            _mapper.Map<IEnumerable<RuleDto>>(aggregationTable),
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

        var rule = await _unitofWork.RuleRepository.GetRule(id, userId, cts);

        if (rule is null)
        {
            return NotFound(ErrorCode[6001]);
        }

        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(rule.IssueTypeId, cts);
        var issueField = await _unitofWork.IssueFieldRepository.GetIssueField(
            rule.IssueFieldId,
            cts
        );

        await _unitofWork.CommitAsync(cts);

        var aggregationTable = new ValueTuple<RuleModel, IssueTypeModel?, IssueFieldModel?>
        {
            Item1 = rule,
            Item2 = issueType ?? null,
            Item3 = issueField ?? null
        };

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(aggregationTable));

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> AddRule([FromBody] RuleRequest request, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
            request.IssueTypeId,
            cts
        );

        if (issueType is null)
        {
            return NotFound(ErrorCode[7001]);
        }

        var issueField = await _unitofWork.IssueFieldRepository.GetIssueField(
            request.IssueFieldId,
            cts
        );

        if (issueField is null)
        {
            return NotFound(ErrorCode[7002]);
        }

        var newRule = _mapper.Map<RuleModel>(request);
        newRule.OwnerUserId = userId;

        var addedRule = await _unitofWork.RuleRepository.AddRule(newRule, cts);
        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "Rule ID {RuleId} was added for user ID {UserId}",
            addedRule.Id,
            addedRule.OwnerUserId
        );

        var aggregationTable = new ValueTuple<RuleModel, IssueTypeModel?, IssueFieldModel?>
        {
            Item1 = addedRule,
            Item2 = issueType,
            Item3 = issueField
        };

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(aggregationTable));

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
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var ruleExist = await _unitofWork.RuleRepository.DoesRuleExist(id, userId, cts);

        if (!ruleExist)
        {
            return NotFound(ErrorCode[6001]);
        }

        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
            request.IssueTypeId,
            cts
        );

        if (issueType is null)
        {
            return NotFound(ErrorCode[7001]);
        }

        var issueField = await _unitofWork.IssueFieldRepository.GetIssueField(
            request.IssueFieldId,
            cts
        );

        if (issueField is null)
        {
            return NotFound(ErrorCode[7002]);
        }

        var rule = _mapper.Map<RuleModel>(request);
        rule.Id = id;
        rule.OwnerUserId = userId;

        var updatedRule = await _unitofWork.RuleRepository.UpdateRule(rule, cts);
        await _unitofWork.CommitAsync(cts);

        _logger.LogInformation(
            "Rule ID {RuleId} was updated for user ID {UserId}",
            updatedRule.Id,
            updatedRule.OwnerUserId
        );

        var aggregationTable = new ValueTuple<RuleModel, IssueTypeModel?, IssueFieldModel?>
        {
            Item1 = updatedRule,
            Item2 = issueType,
            Item3 = issueField
        };

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(aggregationTable));

        return Ok(response);
    }

    [Route("{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteRule(
        [FromRoute] int id,
        [FromBody] RuleRequest request,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var ruleExist = await _unitofWork.RuleRepository.DoesRuleExist(id, userId, cts);

        if (!ruleExist)
        {
            return NotFound(ErrorCode[6001]);
        }

        var deletedRule = await _unitofWork.RuleRepository.DeleteRule(id, userId, cts);

        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
            deletedRule.IssueTypeId,
            cts
        );
        var issueField = await _unitofWork.IssueFieldRepository.GetIssueField(
            deletedRule.IssueFieldId,
            cts
        );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "Rule ID {RuleId} was updated for user ID {UserId}",
            deletedRule.Id,
            deletedRule.OwnerUserId
        );

        var aggregationTable = new ValueTuple<RuleModel, IssueTypeModel?, IssueFieldModel?>
        {
            Item1 = deletedRule,
            Item2 = issueType ?? null,
            Item3 = issueField ?? null
        };

        var response = new Response<RuleDto>(_mapper.Map<RuleDto>(aggregationTable));

        return Ok(response);
    }
}