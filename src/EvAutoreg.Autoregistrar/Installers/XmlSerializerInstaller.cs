using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Options;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace EvAutoreg.Autoregistrar.Installers;

public static class XmlSerializerInstaller
{
    public static void AddExtendedXmlSerializer(
        this WebApplicationBuilder builder,
        XmlIssueOptions options
    )
    {
        var serializer = new ConfigurationContainer()
            .Type<XmlIssue>()
            .Name(options.Root)
            .Member(x => x.Id)
            .Name(options.Id)
            .Member(x => x.TimeCreated)
            .Name(options.TimeCreated)
            .Member(x => x.Author)
            .Name(options.Author)
            .Member(x => x.Company)
            .Name(options.Company)
            .Member(x => x.Status)
            .Name(options.Status)
            .Member(x => x.Priority)
            .Name(options.Priority)
            .Member(x => x.AssignedGroup)
            .Name(options.AssignedGroup)
            .Member(x => x.Assignee)
            .Name(options.Assignee)
            .Member(x => x.ShortDescription)
            .Name(options.ShortDescription)
            .Member(x => x.Description)
            .Name(options.Description)
            .Create();

        builder.Services.AddSingleton(serializer);
    }
}