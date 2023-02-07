using System.Xml.Serialization;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Services.Interfaces;

namespace EvAutoreg.Autoregistrar.Services;

public class IssueDeserialzer : IIssueDeserialzer
{
    private readonly XmlSerializer _serializer;

    public IssueDeserialzer(XmlSerializer serializer)
    {
        _serializer = serializer;
    }

    public XmlIssue DeserializeIssue(string xml)
    {
        using var reader = new StringReader(xml);
        return (XmlIssue)_serializer.Deserialize(reader)!;
    }
}