using Grpc.Core;

namespace Autoregistrar.Services;

public class AutoregistrarService : Autoregistrar.AutoregistrarBase
{
    private static Status CurrentStatus { get; set; } = Status.Stopped;
    private static int ForUser { get; set; }

    public override Task<StatusResponse> StartService(
        StartRequest request,
        ServerCallContext context
    )
    {
        CurrentStatus = Status.Pending;
        Console.WriteLine("Starting service...");
        ForUser = request.UserId;
        CurrentStatus = Status.Started;
        return Task.FromResult(new StatusResponse { Status = CurrentStatus, UserId = ForUser });
    }

    public override Task<StatusResponse> StopService(StopRequest request, ServerCallContext context)
    {
        CurrentStatus = Status.Pending;
        Console.WriteLine("Stopping service...");
        ForUser = default;
        CurrentStatus = Status.Stopped;
        return Task.FromResult(new StatusResponse { Status = CurrentStatus });
    }

    public override Task<StatusResponse> RequestStatus(Empty request, ServerCallContext context)
    {
        return Task.FromResult(
            CurrentStatus == Status.Started
                ? new StatusResponse { Status = CurrentStatus, UserId = ForUser }
                : new StatusResponse { Status = CurrentStatus }
        );
    }
}