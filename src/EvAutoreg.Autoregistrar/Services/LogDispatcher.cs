using EvAutoreg.Autoregistrar.Hubs;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EvAutoreg.Autoregistrar.Services;

public class LogDispatcher<T> : ILogDispatcher<T>
{
    private readonly ILogger<LogDispatcher<T>> _logger;
    private readonly IHubContext<AutoregistrarHub, IAutoregistrarClient> _hubContext;

    public LogDispatcher(ILogger<LogDispatcher<T>> logger, IHubContext<AutoregistrarHub, IAutoregistrarClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Log(string log)
    {
        await _hubContext.Clients.All.ReceiveLog(log);
        _logger.LogInformation("Registering event: {Event}", log);
    }
}