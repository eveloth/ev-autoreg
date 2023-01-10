using System.Net;
using Autoregistrar.Domain;
using Autoregistrar.Settings;
using Extensions;

namespace Autoregistrar.Apis;

public class EvApi : IEvApi
{
    private readonly HttpClient _client;

    public EvApi(HttpClient client)
    {
        _client = client;
    }

    public async Task<EvResponse<XmlIssue?>> GetIssue(string issueNo)
    {
        var query =
            $"https://{GlobalSettings.AutoregistrarSettings.ExtraViewUri}/"
            + $"evj/ExtraView/ev_api.action?user_id={GlobalSettings.ExtraViewCredentials.Email}"
            + $"&password={GlobalSettings.ExtraViewCredentials.Password}&statevar=get&id={issueNo}";



        var response = await _client.GetAsync(query);
        var xmlIssueString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && xmlIssueString.StartsWith("<?xml"))
        {
            return new EvResponse<XmlIssue?>
            {
                StatusCode = HttpStatusCode.OK,
                Content = xmlIssueString.DeserializeXmlString<XmlIssue>()
            };
        }

        return new EvResponse<XmlIssue?> { StatusCode = HttpStatusCode.BadRequest, Content = null };
    }

    public async Task<EvResponse<string>> UpdateIssue(
        string issueNo,
        params string[] queryParameters
    )
    {
        var query =
            $"https://{GlobalSettings.AutoregistrarSettings.ExtraViewUri}/"
            + $"evj/ExtraView/ev_api.action?user_id={GlobalSettings.ExtraViewCredentials.Email}"
            + $"&password={GlobalSettings.ExtraViewCredentials.Password}&statevar=update&id={issueNo}";

        foreach (var param in queryParameters)
        {
            query += $"&{param}";
        }

        var response = await _client.GetAsync(query);
        var responseString = await response.Content.ReadAsStringAsync();

        if (
            response.IsSuccessStatusCode
            && responseString.Contains("updated", StringComparison.InvariantCultureIgnoreCase)
        )
        {
            return new EvResponse<string>
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseString
            };
        }

        return new EvResponse<string>
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = responseString
        };
    }
}