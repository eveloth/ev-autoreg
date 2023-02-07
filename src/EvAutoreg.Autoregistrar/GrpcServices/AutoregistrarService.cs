using System.Security.Cryptography;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Exchange.WebServices.Data;
using Npgsql;
using Task = System.Threading.Tasks.Task;

namespace EvAutoreg.Autoregistrar.GrpcServices;

public class AutoregistrarService : Autoregistrar.AutoregistrarBase
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMailEventListener _listener;
    private readonly ILogDispatcher<AutoregistrarService> _logDispatcher;

    public AutoregistrarService(
        ISettingsProvider settingsProvider,
        IMailEventListener listener,
        ILogDispatcher<AutoregistrarService> logDispatcher
    )
    {
        _settingsProvider = settingsProvider;
        _listener = listener;
        _logDispatcher = logDispatcher;
    }

    [Authorize("UseRegistrar")]
    public override async Task<StatusResponse> StartService(
        StartRequest request,
        ServerCallContext context
    )
    {
        if (!StateManager.IsStopped())
        {
            return new StatusResponse
            {
                RequestStatus = ReqStatus.Failed,
                ServiceStatus = StateManager.GetStatus(),
                UserId = StateManager.GetOperator(),
                Description = "Service is not stopped"
            };
        }

        var serviceOperator = request.UserId;

        StateManager.SetStatus(Status.Pending);
        StateManager.SetOperator(serviceOperator);

        await _logDispatcher.Log($"Starting autoregistrar for user ID {serviceOperator}");

        try
        {
            var areSettingsValid = await _settingsProvider.CheckSettingsIntegrity(
                serviceOperator,
                context.CancellationToken
            );

            if (!areSettingsValid)
            {
                await _logDispatcher.Log("Can't start the autoregistrar, settings are invalid");

                StateManager.SetStatus(Status.Stopped);

                return new StatusResponse
                {
                    RequestStatus = ReqStatus.Failed,
                    ServiceStatus = StateManager.GetStatus(),
                    UserId = StateManager.GetOperator(),
                    Description = "Settings are invalid!"
                };
            }

            await _settingsProvider.InitializeSettings(serviceOperator, context.CancellationToken);
            await _listener.OpenConnection(context.CancellationToken);
        }
        catch (Exception e) when (e is CryptographicException or NpgsqlException)
        {
            await _logDispatcher.Log(
                "Couldn't start service; " + "consult the service administartor"
            );
            StateManager.SetStatus(Status.Stopped);
            throw new RpcException(new Grpc.Core.Status(StatusCode.Internal, e.ToString()));
        }
        catch (Exception e) when (e is ServiceRequestException)
        {
            await _logDispatcher.Log(
                "Couldn't connect to the Exchange server; the reason might be credentials are invalid "
                    + "or Exchange server is unreachable"
            );
            StateManager.SetStatus(Status.Stopped);
            throw new RpcException(
                new Grpc.Core.Status(StatusCode.FailedPrecondition, e.ToString())
            );
        }

        StateManager.SetStatus(Status.Started);

        return new StatusResponse
        {
            RequestStatus = ReqStatus.Success,
            ServiceStatus = StateManager.GetStatus(),
            UserId = StateManager.GetOperator(),
            Description = "Service started!"
        };
    }

    [Authorize("UseRegistrar")]
    public override async Task<StatusResponse> StopService(
        StopRequest request,
        ServerCallContext context
    )
    {
        var serviceOperator = StateManager.GetOperator();

        if (!StateManager.IsOperator(request.UserId))
        {
            return new StatusResponse
            {
                RequestStatus = ReqStatus.Failed,
                ServiceStatus = StateManager.GetStatus(),
                UserId = serviceOperator,
                Description = "Only the operator of the service can stop it."
            };
        }

        StateManager.SetStatus(Status.Pending);

        _settingsProvider.Clear(serviceOperator);
        _listener.CloseConnection();

        await _logDispatcher.Log($"Stopped autoregistrar for user ID {serviceOperator}");

        StateManager.SetOperator(default);
        StateManager.SetStatus(Status.Stopped);

        return new StatusResponse
        {
            RequestStatus = ReqStatus.Success,
            ServiceStatus = StateManager.GetStatus(),
            Description = "Service stopped!"
        };
    }

    [Authorize("ForceStopAutoregistrar")]
    public override async Task<StatusResponse> ForceStopService(
        ForceStopRequest request,
        ServerCallContext context
    )
    {
        var serviceOperator = StateManager.GetOperator();

        StateManager.SetStatus(Status.Pending);

        _settingsProvider.Clear(serviceOperator);
        _listener.CloseConnection();

        await _logDispatcher.Log($"Stopped autoregistrar for user ID {serviceOperator}");

        StateManager.SetOperator(default);
        StateManager.SetStatus(Status.Stopped);

        return new StatusResponse
        {
            RequestStatus = ReqStatus.Success,
            ServiceStatus = StateManager.GetStatus(),
            Description = "Service stopped!"
        };
    }

    [Authorize]
    public override Task<StatusResponse> RequestStatus(Empty request, ServerCallContext context)
    {
        return Task.FromResult(
            StateManager.GetStatus() is Status.Started or Status.Pending
                ? new StatusResponse
                {
                    RequestStatus = ReqStatus.Success,
                    ServiceStatus = StateManager.GetStatus(),
                    UserId = StateManager.GetOperator()
                }
                : new StatusResponse
                {
                    RequestStatus = ReqStatus.Success,
                    ServiceStatus = StateManager.GetStatus()
                }
        );
    }
}