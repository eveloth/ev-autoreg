using System.Text;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Autoregistrar.Apis;

public class EvApi : IEvApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EvApi> _logger;
    private readonly IIssueDeserialzer _issueDeserialzer;

    private const string Https = "https://";
    private const string EvBackend = "/evj/ExtraView/";
    private const string EvAction = "ev_api.action?";
    private const string EvUserId = "user_id=";
    private const string EvPassword = "&password=";
    private const string MethodCall = "&statevar=";
    private const string GetMethod = "get";
    private const string UpdateMethod = "update";
    private const string IssueId = "&id=";

    public static string ClientName => nameof(EvApi) + "Client";

    public EvApi(
        ILogger<EvApi> logger,
        IHttpClientFactory httpClientFactory,
        IIssueDeserialzer issueDeserialzer
    )
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _issueDeserialzer = issueDeserialzer;
    }

    public async Task<XmlIssue> GetIssue(string issueNo)
    {
        var evUri = GlobalSettings.AutoregistrarSettings!.ExtraViewUri;
        var evEmail = GlobalSettings.ExtraViewCredentials!.Email;
        var evPassword = GlobalSettings.ExtraViewCredentials.Password;

        var client = _httpClientFactory.CreateClient(ClientName);
        var queryBuilder = new StringBuilder(Https)
            .Append(evUri)
            .Append(EvBackend)
            .Append(EvAction)
            .Append(EvUserId)
            .Append(evEmail)
            .Append(EvPassword)
            .Append(evPassword)
            .Append(MethodCall)
            .Append(GetMethod)
            .Append(IssueId)
            .Append(issueNo);

        var query = queryBuilder.ToString();

        _logger.LogInformation("Initiating request to EV server at {Uri}", query);

        var response = await client.GetAsync(query);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new EvApiException(responseBody, response.StatusCode);
        }

        if (!IsValidXml(responseBody))
        {
            throw new EvApiException(responseBody);
        }

        return _issueDeserialzer.DeserializeIssue(responseBody);
    }

    public async Task UpdateIssue(string issueNo, params string[] queryParameters)
    {
        var evUri = GlobalSettings.AutoregistrarSettings!.ExtraViewUri;
        var evEmail = GlobalSettings.ExtraViewCredentials!.Email;
        var evPassword = GlobalSettings.ExtraViewCredentials.Password;

        var client = _httpClientFactory.CreateClient(ClientName);
        var queryBuilder = new StringBuilder(Https)
            .Append(evUri)
            .Append(EvBackend)
            .Append(EvAction)
            .Append(EvUserId)
            .Append(evEmail)
            .Append(EvPassword)
            .Append(evPassword)
            .Append(MethodCall)
            .Append(UpdateMethod)
            .Append(IssueId)
            .Append(issueNo);

        foreach (var param in queryParameters)
        {
            if (!param.IsNullOrEmpty())
            {
                queryBuilder.Append('&').Append(param);
            }
        }

        var query = queryBuilder.ToString();

        _logger.LogInformation("Initiating request to EV server at {Uri}", query);

        var response = await client.GetAsync(query);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new EvApiException(responseBody, response.StatusCode);
        }

        if (!responseBody.Contains("updated", StringComparison.OrdinalIgnoreCase))
        {
            throw new EvApiException(responseBody);
        }
    }

    private static bool IsValidXml(string xmlString)
    {
        return xmlString.StartsWith("<?xml");
    }
}