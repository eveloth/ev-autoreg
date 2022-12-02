using Autoregistrar.Services;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Repository.Interfaces;

namespace Autoregistrar.Settings;

public class SettingsProvider : ISettingsProvider
{
    private readonly ILogger<SettingsProvider> _logger;
    private readonly ICredentialsDecryptor _decryptor;
    private readonly IServiceScopeFactory _scopeFactory;

    public SettingsProvider(
        ILogger<SettingsProvider> logger,
        IServiceScopeFactory scopeFactory,
        ICredentialsDecryptor decryptor
    )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _decryptor = decryptor;
    }

    public async Task<Settings> GetSettings(int userId, CancellationToken cts)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitofWork =
            scope.ServiceProvider.GetService<IUnitofWork>() ?? throw new NullReferenceException();

        var settings = new Settings();

        settings.AutoregSettings = await unitofWork.AutoregistrarSettingsRepository.Get(cts);

        var encryptedExchangeCredentials =
            await unitofWork.ExtCredentialsRepository.GetExchangeCredentials(userId, cts);
        settings.ExchangeCredentials = _decryptor.DecryptExchangeCredentials(
            encryptedExchangeCredentials
        );

        var encryptedEvCredentials = await unitofWork.ExtCredentialsRepository.GetEvCredentials(
            userId,
            cts
        );
        settings.ExtraViewCredentials = _decryptor.DecryptEvCredentials(encryptedEvCredentials);

        var issueTypes = await unitofWork.IssueTypeRepository.GetAllIssueTypes(
            new PaginationFilter(1, 1000),
            cts
        );

        foreach (var issueType in issueTypes)
        {
            var queryParams = await unitofWork.EvApiQueryParametersRepository.GetQueryParameters(
                issueType.Id,
                cts
            );

            if (queryParams is not null)
            {
                settings.QueryParameters.Add(queryParams);
            }
        }

        settings.Rules = (
            await unitofWork.RuleRepository.GetAllRules(userId, new PaginationFilter(1, 1000), cts)
        ).ToList();

        await unitofWork.CommitAsync(cts);

        _logger.LogInformation("Retrieved autoregistrar settings for user ID {UserId}", userId);

        return settings;
    }

    public Task Clear(int userId)
    {
        _logger.LogInformation("Cleared autoregistrar settings for user ID {UserId}", userId);
        return Task.FromResult(StatusManager.Settings = null);
    }
}