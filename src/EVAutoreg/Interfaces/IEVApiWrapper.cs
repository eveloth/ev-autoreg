using System.Net;
using Data.Models;

namespace EVAutoreg.Interfaces;

public interface IEVApiWrapper
{
    Task<XmlIssueModel?> GetIssue(string issueNo);
    Task<HttpStatusCode> UpdateIssue(string issueNo, params string[] queryUpdateParameters);
}