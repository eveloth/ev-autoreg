using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain;
using Api.Extensions;
using Api.Services.Interfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExchangeCredentials = Api.Domain.ExchangeCredentials;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
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

    [Authorize(Policy = "UseRegistrar")]
    [Route("exchange")]
    [HttpPost]
    public async Task<IActionResult> SaveExchangeCredentials(
        [FromBody] ExternalCredentialsRequest credentials,
        CancellationToken cts
    )
    {
        await _extCredentialsValidator.ValidateAndThrowAsync(credentials, cts);

        var userId = HttpContext.GetUserId();

        var updatedForUserId = await _extCredentialsService.SaveExchangeCredentials(
            userId,
            _mapper.Map<ExchangeCredentials>(credentials),
            cts
        );

        _logger.LogInformation(
            "EV Credentials were updated for user ID {UserId}",
            updatedForUserId
        );

        var response = new SuccessResponse(true);
        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("ev")]
    [HttpPost]
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