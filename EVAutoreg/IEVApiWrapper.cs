using System.Net;

namespace EVAutoreg;

public interface IEVApiWrapper
{
    Task GetIssue(string issueNo);
    Task<HttpStatusCode> UpdateIssue(string issueNo, params string[] queryUpdateParameters);
}