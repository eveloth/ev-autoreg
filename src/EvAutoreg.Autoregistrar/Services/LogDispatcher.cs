using System.Text.Json;
using EvAutoreg.Autoregistrar.Hubs;
using EvAutoreg.Autoregistrar.Hubs.Entities;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EvAutoreg.Autoregistrar.Services;

public class LogDispatcher<T> : ILogDispatcher<T>
{
    private readonly ILogger<LogDispatcher<T>> _logger;
    private readonly IHubContext<AutoregistrarHub, IAutoregistrarHubClient> _hub;
    private readonly JsonSerializerOptions _serializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public LogDispatcher(
        ILogger<LogDispatcher<T>> logger,
        IHubContext<AutoregistrarHub, IAutoregistrarHubClient> hub
    )
    {
        _logger = logger;
        _hub = hub;
    }

    public async Task DispatchStatus(Status status)
    {
        var message = JsonSerializer.Serialize(
            new LogMessage(LogType.ServiceStatus, Severity.Info, status.ToString()),
            _serializerOptions
        );

        await _hub.Clients.All.ReceiveLogMessage(message);
    }

    public async Task DispatchInternalMessage(string message)
    {
        var json = JsonSerializer.Serialize(
            new LogMessage(LogType.Log, Severity.Internal, message),
            _serializerOptions
        );

        _logger.LogInformation("Registering event: {Event}", message);
        await _hub.Clients.All.ReceiveLogMessage(json);
    }

    public async Task DispatchInfo(string message)
    {
        var json = JsonSerializer.Serialize(
            new LogMessage(LogType.Log, Severity.Info, message),
            _serializerOptions
        );

        _logger.LogInformation("Registering event: {Event}", message);
        await _hub.Clients.All.ReceiveLogMessage(json);
    }

    public async Task DispatchWarning(string message)
    {
        var json = JsonSerializer.Serialize(
            new LogMessage(LogType.Log, Severity.Warning, message),
            _serializerOptions
        );

        _logger.LogInformation("Registering event: {Event}", message);
        await _hub.Clients.All.ReceiveLogMessage(json);
    }

    public async Task DispatchError(string message)
    {
        var json = JsonSerializer.Serialize(
            new LogMessage(LogType.Log, Severity.Error, message),
            _serializerOptions
        );
        _logger.LogInformation("Registering event: {Event}", message);
        await _hub.Clients.All.ReceiveLogMessage(json);
    }

    public async Task DispatchSuccess(string message)
    {
        var json = JsonSerializer.Serialize(
            new LogMessage(LogType.Log, Severity.Success, message),
            _serializerOptions
        );
        _logger.LogInformation("Registering event: {Event}", message);
        await _hub.Clients.All.ReceiveLogMessage(json);
    }
}