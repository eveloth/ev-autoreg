namespace Autoregistrar.Settings;

public interface ISettingsProvider
{
    Task<Settings> GetSettings(int userId, CancellationToken cts);
    Task Clear(int userId);
}