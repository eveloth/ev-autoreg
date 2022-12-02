namespace Autoregistrar.App;

public interface IMailEventListener
{
    Task OpenConnection(CancellationToken cts);
    void CloseConnection();
}