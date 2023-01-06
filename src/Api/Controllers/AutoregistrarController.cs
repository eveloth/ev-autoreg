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

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Tags("Autoregistrar service management")]
public class AutoregistrarController : ControllerBase
{
    private readonly ILogger<AutoregistrarController> _logger;
    private readonly IValidator<AutoregistrarSettingsRequest> _auoregSettingsValidator;
    private readonly IValidator<IssueTypeRequest> _issueTypeValidator;
    private readonly IValidator<QueryParametersRequest> _queryParametersValidator;
    private readonly IMapper _mapper;
    private readonly IAutoregistrarCallerService _autoregistrarCallerService;
    private readonly IAutoregistrarSettingsService _autoregSettingsService;
    private readonly IIssueTypeService _issueTypeService;
    private readonly IIssueFieldService _issueFieldService;
    private readonly IQueryParametersService _queryParametersService;

    public AutoregistrarController(
        ILogger<AutoregistrarController> logger,
        IMapper mapper,
        IAutoregistrarSettingsService autoregSettingsService,
        IIssueTypeService issueTypeService,
        IIssueFieldService issueFieldService,
        IValidator<AutoregistrarSettingsRequest> auoregSettingsValidator,
        IValidator<IssueTypeRequest> issueTypeValidator,
        IValidator<QueryParametersRequest> queryParametersValidator,
        IQueryParametersService queryParametersService,
        IAutoregistrarCallerService autoregistrarCallerService
    )
    {
        _logger = logger;
        _mapper = mapper;
        _autoregSettingsService = autoregSettingsService;
        _issueTypeService = issueTypeService;
        _issueFieldService = issueFieldService;
        _auoregSettingsValidator = auoregSettingsValidator;
        _issueTypeValidator = issueTypeValidator;
        _queryParametersValidator = queryParametersValidator;
        _queryParametersService = queryParametersService;
        _autoregistrarCallerService = autoregistrarCallerService;
    }

    /// <summary>
    /// Opens the autoregistrar session for the current user
    /// </summary>
    /// <response code="200">Opens the autoregistrar session for the current user</response>
    /// <response code="400">If an interaction with the autoregistrar failed</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("start")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> StartAutoregistrar(CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();
        var statusResponse = await _autoregistrarCallerService.Start(userId, cts);
        _logger.LogInformation("User ID {UserId} initiated autoregistrar start request", userId);

        var response = new Response<StatusResponse>(statusResponse);
        return Ok(response);
    }

    /// <summary>
    /// Closes the autoregistrar session for the current user
    /// </summary>
    /// <response code="200">Closes the autoregistrar session for the current user</response>
    /// <response code="400">If an interaction with the autoregistrar failed</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("stop")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> StopAutoregistar(CancellationToken cts)
    {
        var userId = HttpContext.GetUserId();
        var statusResponse = await _autoregistrarCallerService.Stop(userId, cts);
        _logger.LogInformation("User ID {UserId} initiated autoregistrar stop request", userId);

        var response = new Response<StatusResponse>(statusResponse);
        return Ok(response);
    }

    /// <summary>
    /// Closes the autoregistrar session of any user
    /// </summary>
    /// <response code="200">Closes the autoregistrar session of any user</response>
    /// <response code="400">If an interaction with the autoregistrar failed</response>
    [Authorize(Policy = "ForceStopRegistrar")]
    [Route("force-stop")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> ForceStopAutoregistrar(CancellationToken cts)
    {
        var statusResponse = await _autoregistrarCallerService.ForceStop(cts);
        _logger.LogInformation(
            "User ID {UserId} initiated autoregistrar force stop request",
            HttpContext.GetUserId()
        );

        var response = new Response<StatusResponse>(statusResponse);
        return Ok(response);
    }

    /// <summary>
    /// Returns the current status of the autoregistrar
    /// </summary>
    /// <response code="200">Returns the current status of the autoregistrar</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("status")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAutoregistrarStatus(CancellationToken cts)
    {
        var currentStatus = await _autoregistrarCallerService.GetStatus(cts);
        var response = new Response<StatusResponse>(currentStatus);
        return Ok(response);
    }

    /// <summary>
    /// Returns autoregistrar settings
    /// </summary>
    /// <response code="200">Returns autoregistrar settings</response>
    /// <response code="404">If no settings are set</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("settings")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<AutoregistrarSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAutoregistrarSettings(CancellationToken cts)
    {
        var settings = await _autoregSettingsService.Get(cts);

        var response = new Response<AutoregistrarSettingsDto>(
            _mapper.Map<AutoregistrarSettingsDto>(settings)
        );
        return Ok(response);
    }

    /// <summary>
    /// Sets autoregistrar settings
    /// </summary>
    /// <response code="200">Sets autoregistrar settings</response>
    /// <response code="400">If a validation error occured</response>
    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<AutoregistrarSettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
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

    /// <summary>
    /// Returns all issue types in the system
    /// </summary>
    /// <response code="200">Returns all issue types in the system</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<IssueTypeDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
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

    /// <summary>
    /// Returns the requested issue type
    /// </summary>
    /// <response code="200">Returns the requested issue type</response>
    /// <response code="404">If an issue type doesn't exist</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types/{id:int}")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<IssueTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetIssueType([FromRoute] int id, CancellationToken cts)
    {
        var issueType = await _issueTypeService.Get(id, cts);

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(issueType));
        return Ok(response);
    }

    /// <summary>
    /// Adds a new issue type to the system
    /// </summary>
    /// <response code="200">Adds a new issue type to the system</response>
    /// <response code="400">If a validation error occured or if the issue type name is taken</response>
    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<IssueTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
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

    /// <summary>
    /// Renames an issue type
    /// </summary>
    /// <response code="200">Renames an issue type</response>
    /// <response code="400">If a validation error occured or if the issue type name is taken</response>
    /// <response code="404">If an issue type doesn't exist</response>
    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}")]
    [HttpPut]
    [ProducesResponseType(typeof(Response<IssueTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
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

    /// <summary>
    /// Deletes an issue type from the system
    /// </summary>
    /// <response code="200">Deletes an issue type from the system</response>
    /// <response code="404">If an issue type doesn't exist</response>
    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}")]
    [HttpDelete]
    [ProducesResponseType(typeof(Response<IssueTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteIssueType([FromRoute] int id, CancellationToken cts)
    {
        var deletedIssueType = await _issueTypeService.Delete(id, cts);

        _logger.LogInformation("Issue type ID {IssueTypeId} was deleted", deletedIssueType.Id);

        var response = new Response<IssueTypeDto>(_mapper.Map<IssueTypeDto>(deletedIssueType));
        return Ok(response);
    }

    /// <summary>
    /// Returns all issue fields in the system
    /// </summary>
    /// <response code="200">Returns all issue fields in the system</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-fields")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<IssueFieldDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
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

    /// <summary>
    /// Returns all EV API query parameters associated with all issue types
    /// </summary>
    /// <response code="200">Returns all EV API query parameters associated with all issue types</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types/ev-api-query-parameters")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<QueryParametersDto>), StatusCodes.Status200OK)]
    [Produces("application/json")]
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

    /// <summary>
    /// Returns EV API query parameters for the specified issue type
    /// </summary>
    /// <response code="200">Returns EV API query parameters for the specified issue type</response>
    /// <response code="404">If query parameters are not set for that issue type or if an issue type doesn't exist</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpGet]
    [ProducesResponseType(typeof(Response<QueryParametersDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
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

    /// <summary>
    /// Returns EV API query parameters for the specified issue type
    /// </summary>
    /// <response code="200">Returns EV API query parameters for the specified issue type</response>
    /// <response code="400">If a validation error occured</response>
    /// <response code="404">If an issue type doesn't exist</response>
    [Authorize(Policy = "ConfigureRegistrar")]
    [Route("settings/issue-types/{id:int}/ev-api-query-parameters")]
    [HttpPost]
    [ProducesResponseType(typeof(Response<QueryParametersDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
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