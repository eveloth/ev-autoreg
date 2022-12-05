namespace Autoregistrar.Services;

public interface IMailEventListener
{
    Task OpenConnection(CancellationToken cts);
    void CloseConnection();
}