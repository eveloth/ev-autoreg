using Autoregistrar.App;
using Autoregistrar.Settings;
using Grpc.Core;

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
        StatusManager.Status = Status.Pending;
        Console.WriteLine("Starting service...");
        StatusManager.StartedForUserId = request.UserId;

        try
        {
            StatusManager.Settings = await _settingsProvider.GetSettings(StatusManager.StartedForUserId, context.CancellationToken);
            await _listener.OpenConnection(context.CancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            StatusManager.Status = Status.Stopped;
            throw new RpcException(new Grpc.Core.Status(StatusCode.Internal, e.ToString()));
        }

        StatusManager.Status = Status.Started;
        return await Task.FromResult(new StatusResponse { Status = StatusManager.Status, UserId = StatusManager.StartedForUserId });
    }

    public override Task<StatusResponse> StopService(StopRequest request, ServerCallContext context)
    {
        StatusManager.Status = Status.Pending;
        Console.WriteLine("Stopping service...");

        _settingsProvider.Clear(StatusManager.StartedForUserId);
        _listener.CloseConnection();

        StatusManager.StartedForUserId = default;
        StatusManager.Status = Status.Stopped;
        return Task.FromResult(new StatusResponse { Status = StatusManager.Status });
    }

    public override Task<StatusResponse> RequestStatus(Empty request, ServerCallContext context)
    {
        return Task.FromResult(
            StatusManager.Status == Status.Started
                ? new StatusResponse { Status = StatusManager.Status, UserId = StatusManager.StartedForUserId }
                : new StatusResponse { Status = StatusManager.Status }
        );
    }
}