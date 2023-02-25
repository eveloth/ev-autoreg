using EvAutoreg.Autoregistrar.Services.Interfaces;

namespace EvAutoreg.Autoregistrar.State;

public class StateManager : IStateManager
{
    private readonly ILogDispatcher<StateManager> _logDispatcher;

    public StateManager(ILogDispatcher<StateManager> logDispatcher)
    {
        _logDispatcher = logDispatcher;
    }

    public async Task SetStarted()
    {
        const Status status = Status.Started;

        StateRepository.SetStatus(status);
        await _logDispatcher.DispatchStatus(status);
    }

    public async Task SetPending()
    {
        const Status status = Status.Pending;

        StateRepository.SetStatus(status);
        await _logDispatcher.DispatchStatus(status);
    }

    public async Task SetPending(int operatorId)
    {
        const Status status = Status.Pending;
        
        StateRepository.SetOperator(operatorId);
        StateRepository.SetStatus(status);
        await _logDispatcher.DispatchStatus(status);
    }

    public async Task SetStopped()
    {
        const Status status = Status.Stopped;

        StateRepository.SetStatus(status);
        StateRepository.DropOperator();
        await _logDispatcher.DispatchStatus(status);
    }
}