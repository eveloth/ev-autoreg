using System.Security.Cryptography;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using Grpc.Core;
using Microsoft.Exchange.WebServices.Data;
using Npgsql;
using Task = System.Threading.Tasks.Task;

namespace EvAutoreg.Autoregistrar.Services;

public class ListenerProxy : IListenerProxy
{
    private readonly IStateManager _stateManager;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMailEventListener _mailEventListener;
    private readonly ILogDispatcher<ListenerProxy> _logDispatcher;
    private readonly ILogger<ListenerProxy> _logger;

    public ListenerProxy(
        IStateManager stateManager,
        IMailEventListener mailEventListener,
        ILogDispatcher<ListenerProxy> logDispatcher,
        ISettingsProvider settingsProvider,
        ILogger<ListenerProxy> logger
    )
    {
        _stateManager = stateManager;
        _mailEventListener = mailEventListener;
        _logDispatcher = logDispatcher;
        _settingsProvider = settingsProvider;
        _logger = logger;
    }

    public async Task<StatusResponse> StartListen(int operatorId, CancellationToken cts)
    {
        await _stateManager.SetPending(operatorId);
        await _logDispatcher.DispatchInfo($"Starting autoregistrar for user ID {operatorId}");

        var settingsAreValid = await AreSettingsValid(operatorId, cts);

        if (!settingsAreValid)
        {
            await _logDispatcher.DispatchError(
                "Can't start the autoregistrar, settings are invalid"
            );

            await _stateManager.SetStopped();
            return InvalidSettingsResponse();
        }

        await InitializeSettings(operatorId, cts);
        await InvokeListener(cts);
        await _stateManager.SetStarted();
        return StartedResponse();
    }

    public async Task<StatusResponse> StopListen(int operatorId)
    {
        await _stateManager.SetPending();

        await _logDispatcher.DispatchInfo(
            $"Stopping autoregistrar for user ID {StateRepository.GetOperator()}"
        );

        _mailEventListener.CloseConnection();
        _settingsProvider.Clear(operatorId);
        await _stateManager.SetStopped();
        return StoppedRespone();
    }

    private async Task<bool> AreSettingsValid(int operatorId, CancellationToken cts)
    {
        try
        {
            return await _settingsProvider.CheckSettingsIntegrity(operatorId, cts);
        }
        catch (NpgsqlException e)
        {
            await StopAndThrowOnSettingsRetrieval(e);
            return false;
        }
    }

    private async Task InitializeSettings(int operatorId, CancellationToken cts)
    {
        _logger.LogInformation("Initializing settings for user ID {UserId}", operatorId);

        try
        {
            await _settingsProvider.InitializeSettings(operatorId, cts);
        }
        catch (CryptographicException e)
        {
            await StopAndThrowOnSettingsRetrieval(e);
        }
    }

    private async Task InvokeListener(CancellationToken cts)
    {
        try
        {
            await _mailEventListener.OpenConnection(cts);
        }
        catch (ServiceRequestException e)
        {
            await _logDispatcher.DispatchError(
                "Couldn't connect to the Exchange server; the reason might be credentials are invalid "
                    + "or Exchange server is unreachable"
            );
            await _stateManager.SetStopped();

            throw new RpcException(
                new Grpc.Core.Status(StatusCode.FailedPrecondition, e.ToString())
            );
        }
    }

    private async Task StopAndThrowOnSettingsRetrieval(Exception e)
    {
        await _logDispatcher.DispatchError(
            "Couldn't start service; consult the service administrator"
        );
        _logger.LogError("Settings initialization failed: {Error}", e.Message);

        await _stateManager.SetStopped();
        throw new RpcException(new Grpc.Core.Status(StatusCode.Internal, e.ToString()));
    }

    private static StatusResponse InvalidSettingsResponse()
    {
        return new StatusResponse
        {
            RequestStatus = ReqStatus.Failed,
            ServiceStatus = StateRepository.GetStatus(),
            Description = "Settings are invalid"
        };
    }

    private static StatusResponse StartedResponse()
    {
        return new StatusResponse
        {
            RequestStatus = ReqStatus.Success,
            ServiceStatus = StateRepository.GetStatus(),
            UserId = StateRepository.GetOperator(),
            Description = "Service started"
        };
    }

    private static StatusResponse StoppedRespone()
    {
        return new StatusResponse
        {
            RequestStatus = ReqStatus.Success,
            ServiceStatus = StateRepository.GetStatus(),
            Description = "Service stopped"
        };
    }
}