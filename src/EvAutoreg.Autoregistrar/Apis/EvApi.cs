using System.Net;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Extensions;

namespace EvAutoreg.Autoregistrar.Apis;

public class EvApi : IEvApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EvApi> _logger;

    private readonly string _evUri = GlobalSettings.AutoregistrarSettings!.ExtraViewUri;
    private readonly string _evEmail = GlobalSettings.ExtraViewCredentials!.Email;
    private readonly string _evPassword = GlobalSettings.ExtraViewCredentials.Password;

    public static string ClientName => nameof(EvApi) + "Client";

    public EvApi(ILogger<EvApi> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<XmlIssue> GetIssue(string issueNo)
    {
        var client = _httpClientFactory.CreateClient(ClientName);
        var query =
            $"https://{_evUri}/"
            + $"evj/ExtraView/ev_api.action?user_id={_evEmail}"
            + $"&password={_evPassword}&statevar=get&id={issueNo}";

        var response = await client.GetAsync(query);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new EvApiException(responseBody, response.StatusCode);
        }

        if (!responseBody.StartsWith("<?xml)"))
        {
            throw new EvApiException(responseBody);
        }

        return responseBody.DeserializeXmlString<XmlIssue>();
    }

    public async Task UpdateIssue(string issueNo, params string[] queryParameters)
    {
        var client = _httpClientFactory.CreateClient(ClientName);
        var query =
            $"https://{_evUri}/"
            + $"evj/ExtraView/ev_api.action?user_id={_evEmail}"
            + $"&password={_evPassword}&statevar=update&id={issueNo}";

        foreach (var param in queryParameters)
        {
            query += $"&{param}";
        }

        var response = await client.GetAsync(query);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new EvApiException(responseBody, response.StatusCode);
        }

        if (!responseBody.Contains("updated", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new EvApiException(responseBody);
        }
    }
}