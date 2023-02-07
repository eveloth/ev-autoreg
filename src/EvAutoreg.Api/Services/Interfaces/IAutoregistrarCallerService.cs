namespace EvAutoreg.Api.Services.Interfaces;

public interface IAutoregistrarCallerService
{
    Task<StatusResponse> Start(int userId, CancellationToken cts);
    Task<StatusResponse> Stop(int userId, CancellationToken cts);
    Task<StatusResponse> ForceStop(CancellationToken cts);
    Task<StatusResponse> GetStatus(CancellationToken cts);
}