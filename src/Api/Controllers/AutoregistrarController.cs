using Api.Contracts;
using Api.Contracts.Dto;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain;
using Api.Extensions;
using Api.Services.Interfaces;
using FluentValidation;
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
    private readonly Autoregistrar.AutoregistrarClient _grpcClient;
    private readonly IAutoregistrarSettingsService _autoregSettingsService;
    private readonly IIssueTypeService _issueTypeService;
    private readonly IIssueFieldService _issueFieldService;
    private readonly IQueryParametersService _queryParametersService;
    private readonly IValidator<AutoregistrarSettingsRequest> _auoregSettingsValidator;
    private readonly IValidator<IssueTypeRequest> _issueTypeValidator;
    private readonly IValidator<QueryParametersRequest> _queryParametersValidator;

    public AutoregistrarController(
        ILogger<AutoregistrarController> logger,
        IMapper mapper,
        Autoregistrar.AutoregistrarClient grpcClient,
        IAutoregistrarSettingsService autoregSettingsService,
        IIssueTypeService issueTypeService,
        IIssueFieldService issueFieldService,
        IValidator<AutoregistrarSettingsRequest> auoregSettingsValidator,
        IValidator<IssueTypeRequest> issueTypeValidator,
        IValidator<QueryParametersRequest> queryParametersValidator,
        IQueryParametersService queryParametersService
    )
    {
        _logger = logger;
        _mapper = mapper;
        _grpcClient = grpcClient;
        _autoregSettingsService = autoregSettingsService;
        _issueTypeService = issueTypeService;
        _issueFieldService = issueFieldService;
        _auoregSettingsValidator = auoregSettingsValidator;
        _issueTypeValidator = issueTypeValidator;
        _queryParametersValidator = queryParametersValidator;
        _queryParametersService = queryParametersService;
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("start")]
    [HttpPost]
    public async Task<IActionResult> StartAutoregistrar(CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();

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
        var userId = HttpContext.GetUserId();

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
        var settings = await _autoregSettingsService.Get(cts);

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
        await _auoregSettingsValidator.ValidateAndThrowAsync(request, cts);

        var settings = _mapper.Map<AutoregistrarSettings>(request);
        var insertedSettings = await _autoregSettingsService.Add(settings, cts);

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
        var issueTypes = await _issueTypeService.GetAll(pagination, cts);

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
        var issueType = await _issueTypeService.Get(id, cts);

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
        await _issueTypeValidator.ValidateAndThrowAsync(request, cts);

        var issueType = _mapper.Map<IssueType>(request);

        var createdIssueType = await _issueTypeService.Add(issueType, cts);

        _logger.LogInformation(
            "Added issue type ID {IssueTypeId} with name {IssueTypeName}",
            createdIssueType.Id,
            createdIssueType.IssueTypeName
        );

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(createdIssueType));
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
        await _issueTypeValidator.ValidateAndThrowAsync(request, cts);

        var issueType = _mapper.Map<IssueType>(request);
        issueType.Id = id;

        var changedIssueType = await _issueTypeService.Rename(issueType, cts);

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
        var deletedIssueType = await _issueTypeService.Delete(id, cts);

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
        var issueFields = await _issueFieldService.GetAll(pagination, cts);

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
        var queryParameters = await _queryParametersService.GetAll(pagination, cts);

        var response = new PagedResponse<QueryParametersDto>(
            _mapper.Map<IEnumerable<QueryParametersDto>>(queryParameters),
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
        var queryParameters = await _queryParametersService.Get(id, cts);

        var response = new Response<QueryParametersDto>(
            _mapper.Map<QueryParametersDto>(queryParameters)
        );
        return Ok(response);
    }

    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpPost]
    public async Task<IActionResult> UpsertEvApiQueryParametersForIssueType(
        [FromRoute] int id,
        [FromBody] QueryParametersRequest request,
        CancellationToken cts
    )
    {
        await _queryParametersValidator.ValidateAndThrowAsync(request, cts);

        var queryParameters = _mapper.Map<QueryParameters>(request);
        queryParameters.IssueType.Id = id;

        var createdQueryParameters = await _queryParametersService.Upsert(queryParameters, cts);

        var response = new Response<QueryParametersDto>(
            _mapper.Map<QueryParametersDto>(createdQueryParameters)
        );
        return Ok(response);
    }
}