using System.Net;
using System.Text.RegularExpressions;
using Autoregistrar.Apis;
using Autoregistrar.Domain;
using Autoregistrar.Hubs;
using Autoregistrar.Settings;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;

namespace Autoregistrar.Services;

public class IssueProcessor : IIssueProcessor
{
    private readonly ILogger<IssueProcessor> _logger;
    private readonly IMapper _mapper;
    private readonly IEvApi _evapi;
    private readonly IHubContext<AutoregistrarHub, IAutoregistrarClient> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public IssueProcessor(
        ILogger<IssueProcessor> logger,
        IMapper mapper,
        IEvApi evapi,
        IHubContext<AutoregistrarHub, IAutoregistrarClient> hubContext,
        IServiceScopeFactory scopeFactory
    )
    {
        _logger = logger;
        _mapper = mapper;
        _evapi = evapi;
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
    }

    public async Task ProcessEvent(string issueNo)
    {
        var evResponse = await _evapi.GetIssue(issueNo);

        if (evResponse.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogInformation("Couldn't retrieve an issue from EV server");
            await _hubContext.Clients.All.ReceiveLog("Couldn't retrieve an issue from EV server");
            return;
        }

        var xmlIssue = evResponse.Content!;
        var issueFields = StateManager.Settings!.IssueFields;

        foreach (var field in issueFields)
        {
            //We don't want to use reflection if in the end every rule check will be false
            //because there are no rules associated with this issue field, e.g. with issue Author etc.
            if (field.Rules.Count < 1)
            {
                continue;
            }

            var value = xmlIssue.GetType().GetProperty(field.FieldName)!.GetValue(xmlIssue);
            var rules = field.Rules;

            if (value is not string str)
            {
                _logger.LogError(
                    "Parsing error occured for issue ID {IssueId} in field name {FieldName}",
                    issueNo,
                    field.FieldName
                );
                await _hubContext.Clients.All.ReceiveLog(
                    $"Parsing error occured for issue ID {issueNo} in field name {field.FieldName}"
                );

                continue;
            }

            var rulesByIssueType = rules.GroupBy(x => x.IssueTypeId);

            foreach (var ruleSet in rulesByIssueType)
            {
                if (
                    MatchesARule(ruleSet, str) && DoesntMatchANegativeRule(ruleSet, str)
                    || MatchesARegExp(ruleSet, str) && DoesntMatchANegativeRegExp(ruleSet, str)
                )
                {
                    await UpdateAndSaveIssue(ruleSet.Key, xmlIssue.Id);
                }
            }
        }
    }

    //This class's cognitive complexity is higher than desired, but as for now here we have an external infrastructure bottleneck;
    //the Extra View server has it's own set of rules on whether to consider issue properly 'registered'.
    //There rules differ from company to company, so we needed to ensure we reach at least second-level depth of 'updating':
    //the firts update changes issue status to 'regisered', and the second (if needed) -- to 'in work'.
    //If your issue updating rules are standartised enough, you might not need this second update, but it is there in case somebody needs it.
    private async Task UpdateAndSaveIssue(int issueType, string issueNo)
    {
        var queryParameters = StateManager.Settings!.IssueTypes
            .First(x => x.Id == issueType)
            .QueryParameters;

        var isSecondUpdateNeeded = queryParameters.InWorkStatus is not null;

        var queryString = new List<string>
        {
            queryParameters.WorkTime,
            queryParameters.RegStatus,
            queryParameters.AssignedGroup ?? string.Empty,
            queryParameters.RequestType ?? string.Empty
        };

        var firstUpdateResponse = await _evapi.UpdateIssue(issueNo, queryString.ToArray());

        if (firstUpdateResponse.StatusCode == HttpStatusCode.OK)
        {
            if (!isSecondUpdateNeeded)
            {
                await InsertIssueIntoDatabase(issueNo, issueType);
                return;
            }
        }
        else
        {
            _logger.LogError(
                "Failed to update issue ID {IssueId}, EV server response was: {EvResponse}",
                issueNo,
                firstUpdateResponse.Content
            );
        }

        queryString.Remove(queryParameters.RegStatus);
        queryString.Add(queryParameters.InWorkStatus!);

        var secondUpdateResponse = await _evapi.UpdateIssue(issueNo, queryString.ToArray());

        if (secondUpdateResponse.StatusCode == HttpStatusCode.OK)
        {
            await InsertIssueIntoDatabase(issueNo, issueType);
        }
        else
        {
            _logger.LogError(
                "Failed to update issue ID {IssueId}, EV server response was: {EvResponse}",
                issueNo,
                firstUpdateResponse.Content
            );
        }
    }

    private async Task InsertIssueIntoDatabase(string issueNo, int issueType)
    {
        var xmlIssueResponse = await _evapi.GetIssue(issueNo);

        if (xmlIssueResponse.StatusCode == HttpStatusCode.OK)
        {
            var issue = _mapper.Map<IssueModel>(xmlIssueResponse.Content!);
            issue.IssueTypeId = issueType;
            issue.RegistrarId = StateManager.StartedForUserId;

            using var scope = _scopeFactory.CreateScope();
            var unitofWork = scope.ServiceProvider.GetService<IUnitofWork>();
            await unitofWork!.IssueRepository.UpsertIssue(issue, CancellationToken.None);

            _logger.LogInformation("Issue ID {IssueId} was updated", issue.Id);
            await _hubContext.Clients.All.ReceiveLog($"Issue ID {issue.Id} was updated");
        }
        else
        {
            _logger.LogInformation("Couldn't retrieve an issue from EV server");
            await _hubContext.Clients.All.ReceiveLog("Couldn't retrieve an issue from EV server");
        }
    }

    private static bool MatchesARule(IEnumerable<Rule> ruleSet, string input)
    {
        return ruleSet
            .Where(x => x is { IsNegative: false, IsRegex: false })
            .Select(x => x.RuleSubstring)
            .Any(input.Contains);
    }

    private static bool DoesntMatchANegativeRule(IEnumerable<Rule> ruleSet, string input)
    {
        return !ruleSet
            .Where(x => x is { IsNegative: true, IsRegex: false })
            .Select(x => x.RuleSubstring)
            .Any(input.Contains);
    }

    private static bool MatchesARegExp(IEnumerable<Rule> regExpCollection, string input)
    {
        return regExpCollection
            .Where(x => x is { IsNegative: false, IsRegex: true })
            .Select(x => x.RuleSubstring)
            .Any(x => Regex.IsMatch(input, x));
    }

    private static bool DoesntMatchANegativeRegExp(IEnumerable<Rule> regExpCollection, string input)
    {
        return !regExpCollection
            .Where(x => x is { IsNegative: true, IsRegex: true })
            .Select(x => x.RuleSubstring)
            .Any(x => Regex.IsMatch(input, x));
    }
}