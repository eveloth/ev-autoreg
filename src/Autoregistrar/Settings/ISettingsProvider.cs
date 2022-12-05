namespace Autoregistrar.Settings;

public interface ISettingsProvider
{
    Task<Settings> GetSettings(int userId, CancellationToken cts);
    void Clear(int userId);
    Task<bool> CheckSettingsIntegrity(int userId, CancellationToken cts);
}