using EvAutoreg.Api.Domain;

namespace EvAutoreg.Api.Services.Interfaces;

public interface IAutoregistrarSettingsService
{
    Task<AutoregistrarSettings> Get(CancellationToken cts);
    Task<AutoregistrarSettings> Add(AutoregistrarSettings settings, CancellationToken cts);
}