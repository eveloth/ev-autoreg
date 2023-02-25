namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface ILogDispatcher<T>
{
    Task DispatchStatus(Status status);
    Task DispatchInternalMessage(string message);
    Task DispatchInfo(string message);
    Task DispatchWarning(string message);
    Task DispatchError(string message);
    Task DispatchSuccess(string message);
}