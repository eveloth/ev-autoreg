using Api.Exceptions;
using Api.Services.Interfaces;
using Grpc.Core;
using Microsoft.Net.Http.Headers;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

public class AutoregistrarCallerService : IAutoregistrarCallerService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Autoregistrar.AutoregistrarClient _grpcClient;

    public AutoregistrarCallerService(
        IHttpContextAccessor httpContextAccessor,
        Autoregistrar.AutoregistrarClient grpcClient
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _grpcClient = grpcClient;
    }

    public async Task<StatusResponse> Start(int userId, CancellationToken cts)
    {
        var currentStatus = await GetServiceStatus(cts);

        if (currentStatus.Status != Status.Stopped)
        {
            Thrower.ThrowApiException(ErrorCode[11001]);
        }

        var request = new StartRequest { UserId = userId };
        var metadata = GenerateAuthMetadata();

        return await _grpcClient.StartServiceAsync(request, metadata, cancellationToken: cts);
    }

    public async Task<StatusResponse> Stop(int userId, CancellationToken cts)
    {
        var currentStatus = await GetServiceStatus(cts);

        if (currentStatus.Status != Status.Started)
        {
            Thrower.ThrowApiException(ErrorCode[11002]);
        }

        if (currentStatus.UserId != userId)
        {
            Thrower.ThrowApiException(ErrorCode[11003]);
        }

        var request = new StopRequest { UserId = userId };
        var metadata = GenerateAuthMetadata();

        return await _grpcClient.StopServiceAsync(request, metadata, cancellationToken: cts);
    }

    public async Task<StatusResponse> ForceStop(CancellationToken cts)
    {
        var currentStatus = await GetServiceStatus(cts);

        if (currentStatus.Status != Status.Started)
        {
            Thrower.ThrowApiException(ErrorCode[11002]);
        }

        var request = new ForceStopRequest();
        var metadata = GenerateAuthMetadata();

        return await _grpcClient.ForceStopServiceAsync(request, metadata, cancellationToken: cts);
    }

    public async Task<StatusResponse> GetStatus(CancellationToken cts)
    {
        return await GetServiceStatus(cts);
    }

    private async Task<StatusResponse> GetServiceStatus(CancellationToken cts)
    {
        var request = new Empty();
        var metadata = GenerateAuthMetadata();
        return await _grpcClient.RequestStatusAsync(request, metadata, cancellationToken: cts);
    }

    private Metadata GenerateAuthMetadata()
    {
        return new Metadata { { "Authorization", $"Bearer {GetJwtToken()}" } };
    }

    private string GetJwtToken()
    {
        return _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization]
            .ToString()
            .Replace("Bearer ", string.Empty);
    }
}