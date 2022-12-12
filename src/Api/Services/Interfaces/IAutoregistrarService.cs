using Api.Domain;

namespace Api.Services.Interfaces;

public interface IAutoregistrarService
{
    Task<AutoregistrarSettings?> Get(CancellationToken cts);
    Task<AutoregistrarSettings> Add(AutoregistrarSettings settings, CancellationToken cts);
}