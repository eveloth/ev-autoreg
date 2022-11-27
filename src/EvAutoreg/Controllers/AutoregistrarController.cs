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
public class AutoregistrarController : ControllerBase
{
    private readonly ILogger<AutoregistrarController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public AutoregistrarController(
        ILogger<AutoregistrarController> logger,
        IMapper mapper,
        IUnitofWork unitofWork
    )
    {
        _logger = logger;
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    [Route("start")]
    [HttpPost]
    public async Task<IActionResult> StartAutoregistrar(CancellationToken cts)
    {
        return Ok();
    }

    [Route("stop")]
    [HttpPost]
    public async Task<IActionResult> StopAutoregistar(CancellationToken cts)
    {
        return Ok();
    }

    [Route("status")]
    [HttpGet]
    public async Task<IActionResult> GetAutoregistrarStatus(CancellationToken cts)
    {
        return Ok();
    }

    [Route("settings/mail-analysis-rules")]
    [HttpGet]
    public async Task<IActionResult> GetMailAnalysisRules(CancellationToken cts)
    {
        var mailAnalysisRules = await _unitofWork.MailAnalysisRulesRepository.GetMailAnalysisRules(
            cts
        );

        await _unitofWork.CommitAsync(cts);

        if (mailAnalysisRules is null)
        {
            return NotFound(ErrorCode[7003]);
        }

        var response = new Response<MailAnalysisRulesDto>(
            _mapper.Map<MailAnalysisRulesDto>(mailAnalysisRules)
        );

        return Ok(response);
    }

    [Route("settings/mail-analysis-rules")]
    [HttpPost]
    public async Task<IActionResult> AddMailNalysisRules(
        [FromBody] MailAnalysisRulesRequest request,
        CancellationToken cts
    )
    {
        var rules = _mapper.Map<MailAnalysisRuleModel>(request);
        var mailAnalysisRules =
            await _unitofWork.MailAnalysisRulesRepository.UpsertMailAnalysisRules(rules, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("Mail analysis rules were updated");

        var response = new Response<MailAnalysisRulesDto>(
            _mapper.Map<MailAnalysisRulesDto>(mailAnalysisRules)
        );

        return Ok(response);
    }

    [Route("settings/issue-types")]
    [HttpGet]
    public async Task<IActionResult> GetAllIssueTypes(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var issueTypes = await _unitofWork.IssueTypeRepository.GetAllIssueTypes(
            paginationFilter,
            cts
        );

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<IssueTypeDto>(
            _mapper.Map<IEnumerable<IssueTypeDto>>(issueTypes),
            pagination
        );

        return Ok(response);
    }

    [Route("settings/issue-types/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetIssueType([FromRoute] int id, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (issueType is null)
        {
            return NotFound(ErrorCode[7001]);
        }

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(issueType));

        return Ok(response);
    }

    [Route("settings/issue-types")]
    [HttpPost]
    public async Task<IActionResult> AddIssueType(
        [FromBody] IssueTypeRequest request,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesIssueTypeExist(
            request.IssueTypeName,
            cts
        );

        if (issueTypeExists)
        {
            return BadRequest(ErrorCode[7004]);
        }

        var newIssueType = await _unitofWork.IssueTypeRepository.AddIssueType(
            request.IssueTypeName,
            cts
        );
        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "Added issue type ID {IssueTypeId} with name {IssueTypeName}",
            newIssueType.Id,
            newIssueType.IssueTypeName
        );

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(newIssueType));

        return Ok(response);
    }

    [Route("settings/issue-types/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeIssueTypeName(
        [FromRoute] int id,
        [FromBody] IssueTypeRequest request,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesIssueTypeExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var changedIssueType = await _unitofWork.IssueTypeRepository.ChangeIssueTypeName(
            id,
            request.IssueTypeName,
            cts
        );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "Issue type ID {IssueTypeId} name changed to {IssueTypeName}",
            changedIssueType.Id,
            changedIssueType.IssueTypeName
        );

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(changedIssueType));

        return Ok(response);
    }

    [Route("settings/issue-types/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteIssueType([FromRoute] int id, CancellationToken cts)
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesIssueTypeExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var deletedIssueType = await _unitofWork.IssueTypeRepository.DeleteIssueType(id, cts);
        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("Issue type ID {IssueTypeId} was deleted", deletedIssueType.Id);

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(deletedIssueType));

        return Ok(response);
    }

    [Route("settings/issue-fields")]
    [HttpGet]
    public async Task<IActionResult> GetAllIssueFields(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var issueFields = await _unitofWork.IssueFieldRepository.GetAllIssueFields(
            paginationFilter,
            cts
        );
        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<IssueFieldDto>(
            _mapper.Map<IEnumerable<IssueFieldDto>>(issueFields),
            pagination
        );

        return Ok(response);
    }

    [Route("settings/issue-types/ev-api-query-parameters")]
    [HttpGet]
    public async Task<IActionResult> GetAllEvApiQueryParametersForIssueType(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var queryParameters =
            await _unitofWork.EvApiQueryParametersRepository.GetAllQueryParameters(
                paginationFilter,
                cts
            );

        List<ValueTuple<EvApiQueryParametersModel, IssueTypeModel?>> aggregationTable = new();

        foreach (var parameter in queryParameters)
        {
            var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
                parameter.IssueTypeId,
                cts
            );

            aggregationTable.Add(
                new ValueTuple<EvApiQueryParametersModel, IssueTypeModel?>
                {
                    Item1 = parameter,
                    Item2 = issueType
                }
            );
        }

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<EvApiQueryParametersDto>(
            _mapper.Map<IEnumerable<EvApiQueryParametersDto>>(aggregationTable),
            pagination
        );

        return Ok(response);
    }

    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpGet]
    public async Task<IActionResult> GetEvApiQueryParametersForIssueType(
        [FromRoute] int id,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesIssueTypeExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var queryParameters = await _unitofWork.EvApiQueryParametersRepository.GetQueryParameters(
            id,
            cts
        );

        if (queryParameters is null)
        {
            return NotFound(ErrorCode[7005]);
        }

        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
            queryParameters.IssueTypeId,
            cts
        );

        await _unitofWork.CommitAsync(cts);

        var aggregationTable = new ValueTuple<EvApiQueryParametersModel, IssueTypeModel?>
        {
            Item1 = queryParameters,
            Item2 = issueType
        };

        var response = new Response<EvApiQueryParametersDto>(
            _mapper.Map<EvApiQueryParametersDto>(aggregationTable)
        );

        return Ok(response);
    }

    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpPost]
    public async Task<IActionResult> UpsertEvApiQueryParametersForIssueType(
        [FromRoute] int id,
        [FromBody] EvApiQueryParametersRequest request,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesIssueTypeExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var queryParameters = _mapper.Map<EvApiQueryParametersModel>(request);
        queryParameters.IssueTypeId = id;

        var addedQueryParameters =
            await _unitofWork.EvApiQueryParametersRepository.UpsertQueryParameters(
                queryParameters,
                cts
            );

        var issueType = await _unitofWork.IssueTypeRepository.GetIssueType(
            addedQueryParameters.IssueTypeId,
            cts
        );

        await _unitofWork.CommitAsync(cts);

        var aggregationTable = new ValueTuple<EvApiQueryParametersModel, IssueTypeModel?>
        {
            Item1 = addedQueryParameters,
            Item2 = issueType
        };

        var response = new Response<EvApiQueryParametersDto>(
            _mapper.Map<EvApiQueryParametersDto>(aggregationTable)
        );

        return Ok(response);
    }
}