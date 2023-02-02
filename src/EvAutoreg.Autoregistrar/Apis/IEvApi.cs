using EvAutoreg.Autoregistrar.Domain;

namespace EvAutoreg.Autoregistrar.Apis;

public interface IEvApi
{
    Task<XmlIssue> GetIssue(string issueNo);
    Task UpdateIssue(string issueNo, params string[] queryParameters);
}