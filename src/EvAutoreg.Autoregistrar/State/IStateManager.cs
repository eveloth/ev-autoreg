namespace EvAutoreg.Autoregistrar.State;

public interface IStateManager
{
    Task SetStarted();
    Task SetPending();
    Task SetPending(int operatorId);
    Task SetStopped();
}