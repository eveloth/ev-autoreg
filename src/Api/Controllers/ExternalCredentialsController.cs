using System.Security.Claims;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Domain;
using Api.Services.Interfaces;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExternalCredentialsController : ControllerBase
{
    private readonly ILogger<ExternalCredentialsController> _logger;
    private readonly IUnitofWork _unitofWork;
    private readonly ICredentialsEncryptor _credentialsEncryptor;
    private readonly IMapper _mapper;

    public ExternalCredentialsController(
        ILogger<ExternalCredentialsController> logger,
        IUnitofWork unitofWork,
        ICredentialsEncryptor credentialsEncryptor,
        IMapper mapper
    )
    {
        _logger = logger;
        _unitofWork = unitofWork;
        _credentialsEncryptor = credentialsEncryptor;
        _mapper = mapper;
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("ev")]
    [HttpPost]
    public async Task<IActionResult> SaveEvCredentials(
        [FromBody] ExternalCredentialsRequest credentials,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var cypheredCredentials = _credentialsEncryptor.EncryptEvCredentials(
            userId,
            _mapper.Map<EvCredentials>(credentials)
        );

        var evCredentialsUpdatedForId =
            await _unitofWork.ExtCredentialsRepository.SaveEvCredentials(cypheredCredentials, cts);

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "EV Credentials were updated for user ID {UserId}",
            evCredentialsUpdatedForId
        );

        var response = new Response<int>(evCredentialsUpdatedForId);

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("exchange")]
    [HttpPost]
    public async Task<IActionResult> SaveExchangeCredentials(
        [FromBody] ExternalCredentialsRequest credentials,
        CancellationToken cts
    )
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var cypheredCredentials = _credentialsEncryptor.EncryptExchangeCredentials(
            userId,
            _mapper.Map<ExchangeCredentials>(credentials)
        );

        var exchangeCredentialsUpdatedForId =
            await _unitofWork.ExtCredentialsRepository.SaveExchangeCredentials(
                cypheredCredentials,
                cts
            );

        await _unitofWork.CommitAsync(cts);
        _logger.LogInformation(
            "EV Credentials were updated for user ID {UserId}",
            exchangeCredentialsUpdatedForId
        );

        var response = new Response<int>(exchangeCredentialsUpdatedForId);

        return Ok(response);
    }
}