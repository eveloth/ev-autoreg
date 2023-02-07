using EvAutoreg.Api.Extensions;
using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExchangeCredentials = EvAutoreg.Api.Domain.ExchangeCredentials;

namespace EvAutoreg.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Tags("ExtraView and Exchange credentials management")]
public class ExternalCredentialsController : ControllerBase
{
    private readonly ILogger<ExternalCredentialsController> _logger;
    private readonly IMapper _mapper;
    private readonly IExtCredentialsService _extCredentialsService;
    private readonly IValidator<ExternalCredentialsRequest> _extCredentialsValidator;

    public ExternalCredentialsController(
        ILogger<ExternalCredentialsController> logger,
        IMapper mapper,
        IExtCredentialsService extCredentialsService,
        IValidator<ExternalCredentialsRequest> extCredentialsValidator
    )
    {
        _logger = logger;
        _mapper = mapper;
        _extCredentialsService = extCredentialsService;
        _extCredentialsValidator = extCredentialsValidator;
    }

    /// <summary>
    /// Saves Exchange credentials for the current user
    /// </summary>
    /// <response code="200">Saves Exchange credentials for the current user</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("exchange")]
    [HttpPost]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> SaveExchangeCredentials(
        [FromBody] ExternalCredentialsRequest credentials,
        CancellationToken cts
    )
    {
        await _extCredentialsValidator.ValidateAndThrowAsync(credentials, cts);

        var userId = HttpContext.GetUserId();

        var updatedForUserId = await _extCredentialsService.SaveExchangeCredentials(
            userId,
            _mapper.Map<Domain.ExchangeCredentials>(credentials),
            cts
        );

        _logger.LogInformation(
            "EV Credentials were updated for user ID {UserId}",
            updatedForUserId
        );

        var response = new SuccessResponse(true);
        return Ok(response);
    }

    /// <summary>
    /// Saves ExtraView credentials for the current user
    /// </summary>
    /// <response code="200">Saves ExtraView credentials for the current user</response>
    [Authorize(Policy = "UseRegistrar")]
    [Route("ev")]
    [HttpPost]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> SaveEvCredentials(
        [FromBody] ExternalCredentialsRequest credentials,
        CancellationToken cts
    )
    {
        await _extCredentialsValidator.ValidateAndThrowAsync(credentials, cts);

        var userId = HttpContext.GetUserId();

        var updatedForUserId = await _extCredentialsService.SaveEvCredentials(
            userId,
            _mapper.Map<EvCredentials>(credentials),
            cts
        );

        _logger.LogInformation(
            "EV Credentials were updated for user ID {UserId}",
            updatedForUserId
        );

        var response = new SuccessResponse(true);
        return Ok(response);
    }
}