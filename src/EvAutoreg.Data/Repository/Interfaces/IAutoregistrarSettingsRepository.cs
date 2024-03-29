using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IAutoregistrarSettingsRepository
{
    Task<AutoregstrarSettingsModel?> Get(CancellationToken cts);
    Task<AutoregstrarSettingsModel> Upsert(
        AutoregstrarSettingsModel settings,
        CancellationToken cts
    );
    Task<AutoregstrarSettingsModel> Delete(CancellationToken cts);
    Task<bool> DoExist(CancellationToken cts);
}