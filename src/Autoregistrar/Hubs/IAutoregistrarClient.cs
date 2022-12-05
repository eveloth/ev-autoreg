namespace Autoregistrar.Hubs;

public interface IAutoregistrarClient
{
    Task ReceiveLog(string log);
}