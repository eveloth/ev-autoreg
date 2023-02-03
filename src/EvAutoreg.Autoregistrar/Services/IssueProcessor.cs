using EvAutoreg.Autoregistrar.Apis;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;

namespace EvAutoreg.Autoregistrar.Services;

public class IssueProcessor : IIssueProcessor
{
    private readonly IMapper _mapper;
    private readonly IEvApi _evapi;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogDispatcher<IssueProcessor> _logDispatcher;
    private readonly IIssueAnalyzer _issueAnalyzer;

    public IssueProcessor(
        IMapper mapper,
        IEvApi evapi,
        IServiceScopeFactory scopeFactory,
        ILogDispatcher<IssueProcessor> logDispatcher,
        IIssueAnalyzer issueAnalyzer
    )
    {
        _mapper = mapper;
        _evapi = evapi;
        _scopeFactory = scopeFactory;
        _logDispatcher = logDispatcher;
        _issueAnalyzer = issueAnalyzer;
    }

    public async Task ProcessEvent(string issueNo)
    {
        var xmlIssue = await _evapi.GetIssue(issueNo);

        var issueTypeId = await _issueAnalyzer.AnalyzeIssue(xmlIssue);

        if (issueTypeId is null)
        {
            await _logDispatcher.Log($"Issue ID {issueNo} doesn't match any rules, skipping");
            return;
        }

        await SaveAndUpdateIssue(xmlIssue, issueTypeId.Value);
    }

    private async Task SaveAndUpdateIssue(XmlIssue xmlIssue, int issueTypeId)
    {
        var loadedIssueType = GlobalSettings.IssueTypes!.First(x => x.Id == issueTypeId);
        var queryParameters = loadedIssueType.QueryParameters;

        await _logDispatcher.Log($"Registering an issue as: {loadedIssueType.IssueTypeName}");

        await InsertIssueIntoDatabase(xmlIssue, issueTypeId);

        await _logDispatcher.Log($"Inserted issue ID {xmlIssue.Id} into the database");

        await UpdateIssue(queryParameters, xmlIssue, issueTypeId);
    }

    private async Task InsertIssueIntoDatabase(XmlIssue xmlIssue, int issueTypeId)
    {
        var issue = _mapper.Map<IssueModel>(xmlIssue);

        issue.IssueTypeId = issueTypeId;
        issue.RegistrarId = StateManager.GetOperator();

        using var scope = _scopeFactory.CreateScope();
        var unitofWork = scope.ServiceProvider.GetRequiredService<IUnitofWork>();
        await unitofWork.IssueRepository.Upsert(issue, CancellationToken.None);
        await unitofWork.CommitAsync(CancellationToken.None);

        await _logDispatcher.Log($"Issue ID {issue.Id} was updated");
    }

    private async Task UpdateIssue(
        IEnumerable<QueryParameters> queryParameters,
        XmlIssue xmlIssue,
        int issueTypeId
    )
    {
        var sortedQueryParameters = queryParameters.OrderBy(x => x.ExecutionOrder);

        foreach (var qp in sortedQueryParameters)
        {
            var queryString = new[]
            {
                qp.WorkTime,
                qp.Status,
                qp.AssignedGroup ?? string.Empty,
                qp.RequestType ?? string.Empty
            };

            await _evapi.UpdateIssue(xmlIssue.Id, queryString);
            await InsertIssueIntoDatabase(xmlIssue, issueTypeId);
            await _logDispatcher.Log($"Inserted issue ID {xmlIssue.Id} into the database");
        }
    }
}