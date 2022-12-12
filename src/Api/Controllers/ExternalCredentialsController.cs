using System.Security.Claims;
using Api.Contracts.Requests;
using Api.Contracts.Responses;
using Api.Services.Interfaces;
using DataAccessLibrary.Repository.Interfaces;
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

    public ExternalCredentialsController(
        ILogger<ExternalCredentialsController> logger,
        IUnitofWork unitofWork,
        ICredentialsEncryptor credentialsEncryptor
    )
    {
        _logger = logger;
        _unitofWork = unitofWork;
        _credentialsEncryptor = credentialsEncryptor;
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

        var cypheredCredentials = _credentialsEncryptor.EncryptEvCredentials(userId, credentials);

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
            credentials
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