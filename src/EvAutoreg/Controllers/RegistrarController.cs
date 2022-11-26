using System.Security.Claims;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Requests;
using EvAutoreg.Contracts.Responses;
using EvAutoreg.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RegistrarController : ControllerBase
{
    private readonly ILogger<RegistrarController> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;
    private readonly ICredentialsEncryptor _credentialsEncryptor;

    public RegistrarController(
        ILogger<RegistrarController> logger,
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
    [Route("credentials/ev")]
    [HttpPost]
    public async Task<IActionResult> SaveEvCredentials(
        [FromBody] EvCredentialsRequest credentials,
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

    [Obsolete("Added for testing purposes")]
    [Authorize(Policy = "UseRegistrar")]
    [Route("credentials/ev")]
    [HttpGet]
    public async Task<IActionResult> GetEvCredentials(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var encryptedCredentials = await _unitofWork.ExtCredentialsRepository.GetEvCredentials(
            userId,
            cts
        );

        if (encryptedCredentials is null)
        {
            return BadRequest(ErrorCode[5001]);
        }

        var decryptedCredentials = _credentialsEncryptor.DecryptEvCredentials(encryptedCredentials);
        var response = new Response<EvCredentialsDto>(decryptedCredentials);

        return Ok(response);
    }

    [Authorize(Policy = "UseRegistrar")]
    [Route("credentials/exchange")]
    [HttpPost]
    public async Task<IActionResult> SaveExchangeCredentials(
        [FromBody] ExchangeCredentialsRequest credentials,
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

    [Obsolete("Added for testing purposes")]
    [Authorize(Policy = "UseRegistrar")]
    [Route("credentials/exchange")]
    [HttpGet]
    public async Task<IActionResult> GetExchangeCredentials(CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var encryptedCredentials =
            await _unitofWork.ExtCredentialsRepository.GetExchangeCredentials(userId, cts);

        if (encryptedCredentials is null)
        {
            return BadRequest(ErrorCode[5001]);
        }

        var decryptedCredentials = _credentialsEncryptor.DecryptExchangeCredentials(
            encryptedCredentials
        );
        var response = new Response<ExchangeCredentialsDto>(decryptedCredentials);

        return Ok(response);
    }

    /*[Authorize(Policy = "UseRegistrar")]
    [Route("rules")]
    [HttpGet]
    public async Task<IActionResult> GetAllRules([FromQuery] PaginationQuery pagination, CancellationToken cts)
    {
        var userId = int.Parse(
            HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.NameIdentifier)!.Value
        );

        var paginationFilter = pagination.ToFilter();

        var rules = await _unitofWork.RuleRepository.GetAllRules(paginationFilter, userId, cts);
        await _unitofWork.CommitAsync(cts);
        
        var pesponse = new PagedResponse<RuleDto>()
    }*/
}
