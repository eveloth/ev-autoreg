using System.Net;
using EvAutoreg.Console.Data.Models;

namespace EvAutoreg.Console.Interfaces;

public interface IEVApiWrapper
{
    Task<XmlIssueModel?> GetIssue(string issueNo);
    Task<HttpStatusCode> UpdateIssue(string issueNo, params string[] queryUpdateParameters);
}
