using Autoregistrar.Domain;
using Autoregistrar.Services;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;

namespace Autoregistrar.Settings;

public class SettingsProvider : ISettingsProvider
{
    private readonly ILogger<SettingsProvider> _logger;
    private readonly IMapper _mapper;
    private readonly ICredentialsDecryptor _decryptor;
    private readonly IServiceScopeFactory _scopeFactory;

    public SettingsProvider(
        ILogger<SettingsProvider> logger,
        IServiceScopeFactory scopeFactory,
        ICredentialsDecryptor decryptor,
        IMapper mapper
    )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _decryptor = decryptor;
        _mapper = mapper;
    }

    public async Task<Settings> GetSettings(int userId, CancellationToken cts)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitofWork =
            scope.ServiceProvider.GetService<IUnitofWork>() ?? throw new NullReferenceException();

        var autoregSettings = await unitofWork.AutoregistrarSettingsRepository.Get(cts);
        var exchangeCredentials = _decryptor.DecryptExchangeCredentials(
            (await unitofWork.ExtCredentialsRepository.GetExchangeCredentials(userId, cts))!
        );
        var evCredentials = _decryptor.DecryptEvCredentials(
            (await unitofWork.ExtCredentialsRepository.GetEvCredentials(userId, cts))!
        );

        var queryParams = (
            await unitofWork.EvApiQueryParametersRepository.GetAll(
                new PaginationFilter(1, 1000),
                cts
            )
        ).ToList();
        var issueTypeSet = await unitofWork.IssueTypeRepository.GetAll(
            new PaginationFilter(1, 1000),
            cts
        );
        var issueTypes = new List<IssueType>();

        foreach (var type in issueTypeSet)
        {
            var aggregationTable = new ValueTuple<
                IssueTypeModel,
                IEnumerable<EvApiQueryParametersModel>
            >(type, queryParams);
            issueTypes.Add(_mapper.Map<IssueType>(aggregationTable));
        }

        var rules = (
            await unitofWork.RuleRepository.GetAll(userId, new PaginationFilter(1, 1000), cts)
        ).ToList();
        var issueFieldSet = await unitofWork.IssueFieldRepository.GetAll(
            new PaginationFilter(1, 1000),
            cts
        );

        var issueFields = new List<IssueField>();

        foreach (var field in issueFieldSet)
        {
            var aggregationTable = new ValueTuple<IssueFieldModel, IEnumerable<RuleModel>>(
                field,
                rules
            );
            issueFields.Add(_mapper.Map<IssueField>(aggregationTable));
        }

        await unitofWork.CommitAsync(cts);

        _logger.LogInformation("Retrieved autoregistrar settings for user ID {UserId}", userId);

        return new Settings
        {
            AutoregistrarSettings = _mapper.Map<AutoregistrarSettings>(autoregSettings!),
            ExchangeCredentials = exchangeCredentials,
            ExtraViewCredentials = evCredentials,
            IssueFields = issueFields,
            IssueTypes = issueTypes
        };
    }

    public void Clear(int userId)
    {
        _logger.LogInformation("Cleared autoregistrar settings for user ID {UserId}", userId);
        StateManager.Settings = null;
    }

    public async Task<bool> CheckSettingsIntegrity(int userId, CancellationToken cts)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitofWork =
            scope.ServiceProvider.GetService<IUnitofWork>() ?? throw new NullReferenceException();

        var areAutoregSettingsSet = await unitofWork.GpRepository.IsTableEmpty(
            "autoregistrar_settings",
            cts
        );

        if (!areAutoregSettingsSet)
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

        var issueTypes = await unitofWork.IssueTypeRepository.GetAll(
            new PaginationFilter(1, 1000),
            cts
        );

        foreach (var issueType in issueTypes)
        {
            var queryParams = await unitofWork.EvApiQueryParametersRepository.Get(
                issueType.Id,
                cts
            );

            if (queryParams is not null)
            {
                continue;
            }

            await unitofWork.CommitAsync(cts);
            return false;
        }

        return true;
    }
}