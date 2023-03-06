using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;

namespace EvAutoreg.Autoregistrar.Settings;

public class SettingsProvider : ISettingsProvider
{
    private readonly ILogger<SettingsProvider> _logger;
    private readonly IMapper _mapper;
    private readonly ICredentialsDecryptor _decryptor;
    private readonly IServiceScopeFactory _scopeFactory;

    public SettingsProvider(
        ILogger<SettingsProvider> logger,
        IMapper mapper,
        ICredentialsDecryptor credentialsDecryptor,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _mapper = mapper;
        _decryptor = credentialsDecryptor;
        _scopeFactory = serviceScopeFactory;
    }

    public async Task InitializeSettings(int userId, CancellationToken cts)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitofWork = scope.ServiceProvider.GetRequiredService<IUnitofWork>();

        var autoregSettings = await unitofWork.AutoregistrarSettingsRepository.Get(cts);
        var exchangeCredentials = _decryptor.DecryptExchangeCredentials(
            (await unitofWork.ExtCredentialsRepository.GetExchangeCredentials(userId, cts))!
        );
        var evCredentials = _decryptor.DecryptEvCredentials(
            (await unitofWork.ExtCredentialsRepository.GetEvCredentials(userId, cts))!
        );

        var queryParams = (await unitofWork.QueryParametersRepository.GetAll(cts)).ToList();

        var issueTypeSet = await unitofWork.IssueTypeRepository.GetAll(cts);
        var issueTypes = new List<IssueTypeInfo>();

        foreach (var type in issueTypeSet)
        {
            var queryParamsForIssueType = queryParams.Where(x => x.IssueTypeId == type.Id);

            var ruleSetsForIssueType = await unitofWork.RuleSetRepository.GetAllForIssueType(
                userId,
                type.Id,
                CancellationToken.None
            );

            var aggregationTable = new ValueTuple<
                IssueTypeModel,
                IEnumerable<QueryParametersModel>,
                IEnumerable<FilledRuleSetModel>
            >(type, queryParamsForIssueType, ruleSetsForIssueType);

            issueTypes.Add(_mapper.Map<IssueTypeInfo>(aggregationTable));
        }

        var issueFieldsModels = await unitofWork.IssueFieldRepository.GetAll(
            CancellationToken.None
        );
        var issueFields = _mapper.Map<IEnumerable<IssueField>>(issueFieldsModels).ToList();

        await unitofWork.CommitAsync(cts);

        _logger.LogInformation("Retrieved autoregistrar settings for user ID {UserId}", userId);

        GlobalSettings.AutoregistrarSettings = _mapper.Map<AutoregistrarSettings>(autoregSettings!);
        GlobalSettings.ExchangeCredentials = exchangeCredentials;
        GlobalSettings.ExtraViewCredentials = evCredentials;
        GlobalSettings.IssueFields = issueFields;
        GlobalSettings.IssueTypes = issueTypes;
    }

    public void Clear(int userId)
    {
        _logger.LogInformation("Cleared autoregistrar settings for user ID {UserId}", userId);

        GlobalSettings.AutoregistrarSettings = null;
        GlobalSettings.ExchangeCredentials = null;
        GlobalSettings.ExtraViewCredentials = null;
        GlobalSettings.IssueFields = null;
        GlobalSettings.IssueTypes = null;
    }

    public async Task<bool> CheckSettingsIntegrity(int userId, CancellationToken cts)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitofWork = scope.ServiceProvider.GetRequiredService<IUnitofWork>();

        var areAutoregSettingsEmpty = await unitofWork.GpRepository.IsTableEmpty(
            "autoregistrar_settings",
            cts
        );

        if (areAutoregSettingsEmpty)
        {
            return false;
        }

        var exchangeCredentials = await unitofWork.ExtCredentialsRepository.GetExchangeCredentials(
            userId,
            cts
        );
        var evCredentials = await unitofWork.ExtCredentialsRepository.GetEvCredentials(userId, cts);

        if (exchangeCredentials is null || evCredentials is null)
        {
            return false;
        }

        var issueTypes = await unitofWork.IssueTypeRepository.GetAll(cts);

        foreach (var issueType in issueTypes)
        {
            var queryParams = await unitofWork.QueryParametersRepository.Get(issueType.Id, cts);

            if (queryParams.Any())
            {
                continue;
            }

            await unitofWork.CommitAsync(cts);
            return false;
        }

        await unitofWork.CommitAsync(cts);
        return true;
    }
}