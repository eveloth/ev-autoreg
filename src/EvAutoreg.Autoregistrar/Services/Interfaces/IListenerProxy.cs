namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface IListenerProxy
{
    Task<StatusResponse> StartListen(int operatorId, CancellationToken cts);
    Task<StatusResponse> StopListen(int operatorId);
}