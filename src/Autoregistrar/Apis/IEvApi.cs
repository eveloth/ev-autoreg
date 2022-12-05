using Autoregistrar.Domain;

namespace Autoregistrar.Apis;

public interface IEvApi
{
    Task<EvResponse<XmlIssue?>> GetIssue(string issueNo);
    Task<EvResponse<string>> UpdateIssue(string issueNo, params string[] queryParameters);
}