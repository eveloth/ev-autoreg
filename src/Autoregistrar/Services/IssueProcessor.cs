using System.Net;
using Autoregistrar.Apis;
using Autoregistrar.Hubs;
using Autoregistrar.Settings;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;

namespace Autoregistrar.Services;

public class IssueProcessor : IIssueProcessor
{
    private readonly ILogger<IssueProcessor> _logger;
    private readonly IMapper _mapper;
    private readonly IEvApi _evapi;
    private readonly IHubContext<AutoregistrarHub, IAutoregistrarClient> _hubContext;

    public IssueProcessor(
        ILogger<IssueProcessor> logger,
        IMapper mapper,
        IEvApi evapi,
        IHubContext<AutoregistrarHub, IAutoregistrarClient> hubContext
    )
    {
        _logger = logger;
        _mapper = mapper;
        _evapi = evapi;
        _hubContext = hubContext;
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

        var issueFields = StateManager.Settings!.IssueFields;

        foreach (var field in issueFields)
        {
            if (field.Rules.Count < 1)
            {
                continue;
            }

            var property = evResponse.GetType().GetProperty(field.FieldName);
            var value = property!.GetValue(evResponse.Content);

            var rules = field.Rules
                .Where(x => x is { IsNegative: false, IsRegex: false })
                .Select(x => x.RuleSubstring)
                .ToList();

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

            if (rules.Any(str.Contains))
            {
                return;
            }
        }
    }
}