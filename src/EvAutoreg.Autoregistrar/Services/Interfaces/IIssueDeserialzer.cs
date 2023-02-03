using EvAutoreg.Autoregistrar.Domain;

namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface IIssueDeserialzer
{
    XmlIssue DeserializeIssue(string xml);
}