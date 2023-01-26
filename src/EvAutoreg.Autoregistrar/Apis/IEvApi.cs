using EvAutoreg.Autoregistrar.Domain;

namespace EvAutoreg.Autoregistrar.Apis;

public interface IEvApi
{
    Task<EvResponse<XmlIssue?>> GetIssue(string issueNo);
    Task<EvResponse<string>> UpdateIssue(string issueNo, params string[] queryParameters);
}