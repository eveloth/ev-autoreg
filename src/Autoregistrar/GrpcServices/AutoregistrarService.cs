using Autoregistrar.App;
using Autoregistrar.Settings;
using Grpc.Core;
using Task = System.Threading.Tasks.Task;

namespace Autoregistrar.GrpcServices;

public class AutoregistrarService : Autoregistrar.AutoregistrarBase
{
    private readonly ISettingsProvider _settingsProvider;
    private readonly IMailEventListener _listener;

    public AutoregistrarService(ISettingsProvider settingsProvider, IMailEventListener listener)
    {
        _settingsProvider = settingsProvider;
        _listener = listener;
    }

    public override async Task<StatusResponse> StartService(
        StartRequest request,
        ServerCallContext context
    )
    {
        StateManager.Status = Status.Pending;
        Console.WriteLine("Starting service...");
        StateManager.StartedForUserId = request.UserId;

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

    public override Task<StatusResponse> StopService(StopRequest request, ServerCallContext context)
    {
        StateManager.Status = Status.Pending;
        Console.WriteLine("Stopping service...");

        _settingsProvider.Clear(StateManager.StartedForUserId);
        _listener.CloseConnection();

        StateManager.StartedForUserId = default;
        StateManager.Status = Status.Stopped;
        return Task.FromResult(new StatusResponse { Status = StateManager.Status });
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