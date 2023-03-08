namespace EvAutoreg.Autoregistrar.Hubs;

public interface IAutoregistrarHubClient
{
    Task ReceiveLog(string log);
    Task ReceiveLogMessage(string message);
}