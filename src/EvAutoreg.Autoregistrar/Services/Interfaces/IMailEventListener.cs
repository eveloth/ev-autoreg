namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface IMailEventListener
{
    Task OpenConnection(CancellationToken cts);
    void CloseConnection();
}