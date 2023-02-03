using System.Text;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace EvAutoreg.Autoregistrar.Apis;

public class EvApi : IEvApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EvApi> _logger;

    private readonly string _evUri = GlobalSettings.AutoregistrarSettings!.ExtraViewUri;
    private readonly string _evEmail = GlobalSettings.ExtraViewCredentials!.Email;
    private readonly string _evPassword = GlobalSettings.ExtraViewCredentials.Password;

    private const string Https = "https://";
    private const string EvBackend = "/evj/ExtraView/";
    private const string EvAction = "ev.api.action?";
    private const string EvUserId = "user_id=";
    private const string EvPassword = "&password=";
    private const string MethodCall = "&statevar=";
    private const string GetMethod = "get";
    private const string UpdateMethod = "update";
    private const string IssueId = "&id=";

    public static string ClientName => nameof(EvApi) + "Client";

    public EvApi(ILogger<EvApi> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<XmlIssue> GetIssue(string issueNo)
    {
        var client = _httpClientFactory.CreateClient(ClientName);
        var queryBuilder = new StringBuilder(Https)
            .Append(_evUri)
            .Append(EvBackend)
            .Append(EvAction)
            .Append(EvUserId)
            .Append(_evEmail)
            .Append(EvPassword)
            .Append(_evPassword)
            .Append(MethodCall)
            .Append(GetMethod)
            .Append(IssueId)
            .Append(issueNo);

        var query = queryBuilder.ToString();

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

        return responseBody.DeserializeXmlString<XmlIssue>();
    }

    public async Task UpdateIssue(string issueNo, params string[] queryParameters)
    {
        var client = _httpClientFactory.CreateClient(ClientName);
        var queryBuilder = new StringBuilder(Https)
            .Append(_evUri)
            .Append(EvBackend)
            .Append(EvAction)
            .Append(EvUserId)
            .Append(_evEmail)
            .Append(EvPassword)
            .Append(_evPassword)
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