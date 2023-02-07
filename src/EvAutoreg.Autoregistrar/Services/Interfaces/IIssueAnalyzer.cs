using EvAutoreg.Autoregistrar.Domain;

namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface IIssueAnalyzer
{
    Task<int?> AnalyzeIssue(XmlIssue issue);
}