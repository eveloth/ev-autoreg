using System.Security.Claims;
using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Api.Errors.ErrorCodes;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AutoregistrarController : ControllerBase
{
    private readonly ILogger<AutoregistrarController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;
    private readonly Autoregistrar.AutoregistrarClient _grpcClient;

    public AutoregistrarController(
        ILogger<AutoregistrarController> logger,
        IMapper mapper,
        IUnitofWork unitofWork,
        Autoregistrar.AutoregistrarClient grpcClient
    )
    {
        _logger = logger;
        _mapper = mapper;
        _unitofWork = unitofWork;
        _grpcClient = grpcClient;
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("start")]
    [HttpPost]
    public async Task<IActionResult> StartAutoregistrar(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var currentStatus = await _grpcClient.RequestStatusAsync(
            new Empty(),
            cancellationToken: cts
        );

        if (currentStatus.Status != Status.Stopped)
        {
            return BadRequest(ErrorCode[8001]);
        }

        var statusResponse = await _grpcClient.StartServiceAsync(
            new StartRequest { UserId = userId },
            cancellationToken: cts
        );

        var response = new Response<StatusResponse>(statusResponse);

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("stop")]
    [HttpPost]
    public async Task<IActionResult> StopAutoregistar(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var currentStatus = await _grpcClient.RequestStatusAsync(
            new Empty(),
            cancellationToken: cts
        );

        if (currentStatus.Status != Status.Started)
        {
            return BadRequest(ErrorCode[8002]);
        }

        if (currentStatus.UserId != userId)
        {
            return BadRequest(ErrorCode[8003]);
        }

        var statusResponse = await _grpcClient.StopServiceAsync(
            new StopRequest { UserId = userId },
            cancellationToken: cts
        );

        var response = new Response<StatusResponse>(statusResponse);

        return Ok(response);
    }

    [Authorize(Policy = "ForceStopRegistrar")]
    [Route("force-stop")]
    [HttpPost]
    public async Task<IActionResult> ForceStopAutoregistrar(CancellationToken cts)
    {
        var currentStatus = await _grpcClient.RequestStatusAsync(
            new Empty(),
            cancellationToken: cts
        );

        if (currentStatus.Status != Status.Started)
        {
            return BadRequest(ErrorCode[8002]);
        }

        var statusResponse = await _grpcClient.StopServiceAsync(
            new StopRequest(),
            cancellationToken: cts
        );

        var response = new Response<StatusResponse>(statusResponse);

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("status")]
    [HttpGet]
    public async Task<IActionResult> GetAutoregistrarStatus(CancellationToken cts)
    {
        var currentStatus = await _grpcClient.RequestStatusAsync(
            new Empty(),
            cancellationToken: cts
        );

        var response = new Response<StatusResponse>(currentStatus);

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("settings")]
    [HttpGet]
    public async Task<IActionResult> GetAutoregistrarSettings(CancellationToken cts)
    {
        var settings = await _unitofWork.AutoregistrarSettingsRepository.Get(cts);

        await _unitofWork.CommitAsync(cts);

        if (settings is null)
        {
            return NotFound(ErrorCode[7003]);
        }

        var response = new Response<AutoregistrarSettingsDto>(
            _mapper.Map<AutoregistrarSettingsDto>(settings)
        );

        return Ok(response);
    }

    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings")]
    [HttpPost]
    public async Task<IActionResult> AddAutoregistrarSettings(
        [FromBody] AutoregistrarSettingsRequest request,
        CancellationToken cts
    )
    {
        var settings = _mapper.Map<AutoregstrarSettingsModel>(request);
        var insertedSettings = await _unitofWork.AutoregistrarSettingsRepository.Upsert(
            settings,
            cts
        );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("Autoregistrar settings were set");

        var response = new Response<AutoregistrarSettingsDto>(
            _mapper.Map<AutoregistrarSettingsDto>(insertedSettings)
        );

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types")]
    [HttpGet]
    public async Task<IActionResult> GetAllIssueTypes(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var issueTypes = await _unitofWork.IssueTypeRepository.GetAll(paginationFilter, cts);

        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<IssueTypeDto>(
            _mapper.Map<IEnumerable<IssueTypeDto>>(issueTypes),
            pagination
        );

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types/{id:int}")]
    [HttpGet]
    public async Task<IActionResult> GetIssueType([FromRoute] int id, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (issueType is null)
        {
            return NotFound(ErrorCode[7001]);
        }

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(issueType));

        return Ok(response);
    }

    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types")]
    [HttpPost]
    public async Task<IActionResult> AddIssueType(
        [FromBody] IssueTypeRequest request,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesExist(
            request.IssueTypeName,
            cts
        );

        if (issueTypeExists)
        {
            return BadRequest(ErrorCode[7004]);
        }

        var newIssueType = await _unitofWork.IssueTypeRepository.Add(request.IssueTypeName, cts);
        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "Added issue type ID {IssueTypeId} with name {IssueTypeName}",
            newIssueType.Id,
            newIssueType.IssueTypeName
        );

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(newIssueType));

        return Ok(response);
    }

    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> ChangeIssueTypeName(
        [FromRoute] int id,
        [FromBody] IssueTypeRequest request,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var changedIssueType = await _unitofWork.IssueTypeRepository.ChangeName(
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

    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteIssueType([FromRoute] int id, CancellationToken cts)
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var deletedIssueType = await _unitofWork.IssueTypeRepository.Delete(id, cts);
        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation("Issue type ID {IssueTypeId} was deleted", deletedIssueType.Id);

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(deletedIssueType));

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-fields")]
    [HttpGet]
    public async Task<IActionResult> GetAllIssueFields(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var issueFields = await _unitofWork.IssueFieldRepository.GetAll(paginationFilter, cts);
        await _unitofWork.CommitAsync(cts);

        var response = new PagedResponse<IssueFieldDto>(
            _mapper.Map<IEnumerable<IssueFieldDto>>(issueFields),
            pagination
        );

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types/ev-api-query-parameters")]
    [HttpGet]
    public async Task<IActionResult> GetAllEvApiQueryParametersForIssueType(
        [FromQuery] PaginationQuery pagination,
        CancellationToken cts
    )
    {
        var paginationFilter = _mapper.Map<PaginationFilter>(pagination);

        var queryParameters = await _unitofWork.EvApiQueryParametersRepository.GetAll(
            paginationFilter,
            cts
        );

        List<ValueTuple<EvApiQueryParametersModel, IssueTypeModel?>> aggregationTable = new();

        foreach (var parameter in queryParameters)
        {
            var issueType = await _unitofWork.IssueTypeRepository.Get(parameter.IssueTypeId, cts);

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

    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpGet]
    public async Task<IActionResult> GetEvApiQueryParametersForIssueType(
        [FromRoute] int id,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var queryParameters = await _unitofWork.EvApiQueryParametersRepository.Get(id, cts);

        if (queryParameters is null)
        {
            return NotFound(ErrorCode[7005]);
        }

        var issueType = await _unitofWork.IssueTypeRepository.Get(queryParameters.IssueTypeId, cts);

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

    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpPost]
    public async Task<IActionResult> UpsertEvApiQueryParametersForIssueType(
        [FromRoute] int id,
        [FromBody] EvApiQueryParametersRequest request,
        CancellationToken cts
    )
    {
        var issueTypeExists = await _unitofWork.IssueTypeRepository.DoesExist(id, cts);

        if (!issueTypeExists)
        {
            return NotFound(ErrorCode[7001]);
        }

        var queryParameters = _mapper.Map<EvApiQueryParametersModel>(request);
        queryParameters.IssueTypeId = id;

        var addedQueryParameters = await _unitofWork.EvApiQueryParametersRepository.Upsert(
            queryParameters,
            cts
        );

        var issueType = await _unitofWork.IssueTypeRepository.Get(
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