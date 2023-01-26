namespace EvAutoreg.Autoregistrar.Settings;

public interface ISettingsProvider
{
    Task InitializeSettings(int userId, CancellationToken cts);
    void Clear(int userId);
    Task<bool> CheckSettingsIntegrity(int userId, CancellationToken cts);
}