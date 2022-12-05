using Autoregistrar.App;
using Autoregistrar.Hubs;
using Autoregistrar.Settings;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Task = System.Threading.Tasks.Task;

namespace Autoregistrar.GrpcServices;

public class AutoregistrarService : Autoregistrar.AutoregistrarBase
{
    private readonly ILogger<AutoregistrarService> _logger;
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMailEventListener _listener;
    private readonly IHubContext<AutoregistrarHub, IAutoregistrarClient> _hubContext;

    public AutoregistrarService(
        ISettingsProvider settingsProvider,
        IMailEventListener listener,
        IHubContext<AutoregistrarHub, IAutoregistrarClient> hubContext,
        ILogger<AutoregistrarService> logger
    )
    {
        _settingsProvider = settingsProvider;
        _listener = listener;
        _hubContext = hubContext;
        _logger = logger;
    }

    public override async Task<StatusResponse> StartService(
        StartRequest request,
        ServerCallContext context
    )
    {
        StateManager.Status = Status.Pending;
        StateManager.StartedForUserId = request.UserId;
        _logger.LogInformation(
            "Starting autoregistrar for user ID {UserId}",
            StateManager.StartedForUserId
        );
        await _hubContext.Clients.All.ReceiveLog(
            $"Starting autoregistrar for user ID {StateManager.StartedForUserId}"
        );

        try
        {
            var areSettingsValid = await _settingsProvider.CheckSettingsIntegrity(
                StateManager.StartedForUserId,
                context.CancellationToken
            );

            if (!areSettingsValid)
            {
                throw new RpcException(
                    new Grpc.Core.Status(
                        StatusCode.FailedPrecondition,
                        "Autoregistrar configuraion is not valid, make sure all necessary settings are set "
                        + "and each issue type has it's query parameters"
                    )
                );
            }

            StateManager.Settings = await _settingsProvider.GetSettings(
                StateManager.StartedForUserId,
                context.CancellationToken
            );
            await _listener.OpenConnection(context.CancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            StateManager.Status = Status.Stopped;
            throw new RpcException(new Grpc.Core.Status(StatusCode.Internal, e.ToString()));
        }

        StateManager.Status = Status.Started;

        return new StatusResponse
        {
            Status = StateManager.Status,
            UserId = StateManager.StartedForUserId
        };
    }

    public override async Task<StatusResponse> StopService(StopRequest request, ServerCallContext context)
    {
        StateManager.Status = Status.Pending;

        _settingsProvider.Clear(StateManager.StartedForUserId);
        _listener.CloseConnection();

        await _hubContext.Clients.All.ReceiveLog(
            $"Stopped autoregistrar for user ID {StateManager.StartedForUserId}"
        );
        _logger.LogInformation(
            "Stopped autoregistrar for user ID {UserId}",
            StateManager.StartedForUserId
        );

        StateManager.StartedForUserId = default;
        StateManager.Status = Status.Stopped;
        return new StatusResponse { Status = StateManager.Status };
    }

    public override Task<StatusResponse> RequestStatus(Empty request, ServerCallContext context)
    {
        return Task.FromResult(
            StateManager.Status == Status.Started
                ? new StatusResponse
                {
                    Status = StateManager.Status,
                    UserId = StateManager.StartedForUserId
                }
                : new StatusResponse { Status = StateManager.Status }
        );
    }
}