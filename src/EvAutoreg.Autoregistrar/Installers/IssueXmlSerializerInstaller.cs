using System.Xml.Serialization;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Options;

namespace EvAutoreg.Autoregistrar.Installers;

public static class IssueXmlSerializerInstaller
{
    public static void AddIssueXmlSerializer(
        this WebApplicationBuilder builder,
        XmlIssueOptions options
    )
    {
        var overrides = new XmlAttributeOverrides();

        overrides.Add(
            typeof(XmlIssue),
            new XmlAttributes { XmlRoot = new XmlRootAttribute { ElementName = options.Root} }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Id),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Id) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.TimeCreated),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.TimeCreated) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Author),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Author) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Company),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Company) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Status),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Status) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Priority),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Priority) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.AssignedGroup),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.AssignedGroup) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Assignee),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Assignee) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.ShortDescription),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.ShortDescription) } }
        );
        overrides.Add(
            typeof(XmlIssue),
            nameof(XmlIssue.Description),
            new XmlAttributes { XmlElements = { new XmlElementAttribute(options.Description) } }
        );

        var serializer = new XmlSerializer(typeof(XmlIssue), overrides);
        builder.Services.AddSingleton(serializer);
    }
}