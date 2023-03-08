using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.State;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Task = System.Threading.Tasks.Task;

namespace EvAutoreg.Autoregistrar.GrpcServices;

public class AutoregistrarService : Autoregistrar.AutoregistrarBase
{
    private readonly IListenerProxy _listenerProxy;

    public AutoregistrarService(IListenerProxy listenerProxy)
    {
        _listenerProxy = listenerProxy;
    }

    [Authorize("UseRegistrar")]
    public override async Task<StatusResponse> StartService(
        StartRequest request,
        ServerCallContext context
    )
    {
        if (ServiceIsActive())
        {
            return new StatusResponse
            {
                RequestStatus = ReqStatus.Failed,
                ServiceStatus = StateRepository.GetStatus(),
                UserId = StateRepository.GetOperator(),
                Description = "Service is not stopped"
            };
        }

        var response = await _listenerProxy.StartListen(request.UserId, context.CancellationToken);
        return response;
    }

    [Authorize("UseRegistrar")]
    public override async Task<StatusResponse> StopService(
        StopRequest request,
        ServerCallContext context
    )
    {
        if (!RequestInitiatorIsOperator(request.UserId))
        {
            return new StatusResponse
            {
                RequestStatus = ReqStatus.Failed,
                ServiceStatus = StateRepository.GetStatus(),
                UserId = StateRepository.GetOperator(),
                Description = "Only the operator of the service can stop it"
            };
        }

        var response = await _listenerProxy.StopListen(request.UserId);
        return response;
    }

    [Authorize("ForceStopAutoregistrar")]
    public override async Task<StatusResponse> ForceStopService(
        ForceStopRequest request,
        ServerCallContext context
    )
    {
        var response = await _listenerProxy.StopListen(StateRepository.GetOperator());
        return response;
    }

    [Authorize]
    public override Task<StatusResponse> RequestStatus(Empty request, ServerCallContext context)
    {
        return Task.FromResult(
            StateRepository.GetStatus() is Status.Started or Status.Pending
                ? new StatusResponse
                {
                    RequestStatus = ReqStatus.Success,
                    ServiceStatus = StateRepository.GetStatus(),
                    UserId = StateRepository.GetOperator(),
                    Description = "Service is active"
                }
                : new StatusResponse
                {
                    RequestStatus = ReqStatus.Success,
                    ServiceStatus = StateRepository.GetStatus(),
                    Description = "Service is inactive"
                }
        );
    }

    private static bool ServiceIsActive()
    {
        return !StateRepository.IsStopped();
    }

    private static bool RequestInitiatorIsOperator(int operatorId)
    {
        return StateRepository.IsOperator(operatorId);
    }
}