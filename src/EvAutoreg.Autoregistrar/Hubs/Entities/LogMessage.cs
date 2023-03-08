namespace EvAutoreg.Autoregistrar.Hubs.Entities;

public class LogMessage
{
    public LogType LogType { get; init; }
    public Severity Severity { get; init; }
    public string? Message { get; init; }

    public LogMessage(LogType logType, Severity severity)
    {
        LogType = logType;
        Severity = severity;
    }
    public LogMessage(LogType logType, Severity severity, string message)
    {
        LogType = logType;
        Severity = severity;
        Message = message;
    }
}