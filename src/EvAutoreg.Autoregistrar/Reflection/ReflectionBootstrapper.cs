using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Options;

namespace EvAutoreg.Autoregistrar.Reflection;

public static class ReflectionBootstrapper
{
    public static void GatherPropertiesInformation(this WebApplicationBuilder builder)
    {
        var xmlIssueOptionsProperties = typeof(XmlIssueOptions).GetProperties();
        var xmlIssueProperties = typeof(XmlIssue).GetProperties();

        var issuePropertyInfos = new IssuePropertyInfos(
            xmlIssueOptionsProperties,
            xmlIssueProperties
        );

        builder.Services.AddSingleton(issuePropertyInfos);
    }
}